
#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include <stdio.h>
#include <stdlib.h>
#include "kmeans.cuh"
#include <io.h>
//#include <sys/types.h>  
//#include <sys/stat.h>
#include <fcntl.h>
//#include <iostream>
#include <string.h> 
#include <time.h>
#define MAX_CHAR_PER_LINE 128

float** readFromFile(int   isBinaryFile,  // flag: 0 or 1 
	char *filename,      // input file name 
	int  *numObjs,       // num of data objects (local) 
	int  *numCoords)     // num of coordinates 
{
	float **objects;
	int     i, j, len;
	size_t numBytesRead;

	if (isBinaryFile) 
	{  // input file is in raw binary format
		int infile;
		if ((infile = open(filename, O_RDONLY, "0600")) == -1) 
		{
			fprintf(stderr, "Error: no such file (%s)\n", filename);
			return NULL;
		}
		numBytesRead = read(infile, numObjs, sizeof(int));
		assert(numBytesRead == sizeof(int));
		numBytesRead = read(infile, numCoords, sizeof(int));
		assert(numBytesRead == sizeof(int));
		
		printf("File %s numObjs   = %d\n", filename, *numObjs);
		printf("File %s numCoords = %d\n", filename, *numCoords);
		

		// allocate space for objects[][] and read all objects 
		len = (*numObjs) * (*numCoords);
		objects = (float**)malloc((*numObjs) * sizeof(float*));
		assert(objects != NULL);
		objects[0] = (float*)malloc(len * sizeof(float));
		assert(objects[0] != NULL);
		//assign the proper pointer to each float* array
		for (i = 1; i < (*numObjs); i++)
		{
			objects[i] = objects[i - 1] + (*numCoords);
		}			
		numBytesRead = read(infile, objects[0], len * sizeof(float));
		assert(numBytesRead == len * sizeof(float));

		close(infile);
	}
	else 
	{  
		// input file is in ASCII format
		FILE *infile;
		char *line, *ret;
		int   lineLen;

		if ((infile = fopen(filename, "r")) == NULL) 
		{
			fprintf(stderr, "Error: no such file (%s)\n", filename);
			return NULL;
		}

		/* first find the number of objects */
		lineLen = MAX_CHAR_PER_LINE;
		line = (char*)malloc(lineLen);
		assert(line != NULL);

		(*numObjs) = 0;
		while (fgets(line, lineLen, infile) != NULL) 
		{
			/* check each line to find the max line length */
			while (strlen(line) == lineLen - 1) 
			{
				/* this line read is not complete */
				len = strlen(line);
				fseek(infile, -len, SEEK_CUR);

				/* increase lineLen */
				lineLen += MAX_CHAR_PER_LINE;
				line = (char*)realloc(line, lineLen);
				assert(line != NULL);

				ret = fgets(line, lineLen, infile);
				assert(ret != NULL);
			}

			if (strtok(line, " \t\n") != 0)
				(*numObjs)++;
		}
		rewind(infile);
		printf("lineLen = %d\n", lineLen);

		// find the num coordinates for each object 
		(*numCoords) = 0;
		while (fgets(line, lineLen, infile) != NULL) 
		{
			if (strtok(line, " \t\n") != 0) 
			{
				/* ignore the id (first coordiinate): numCoords = 1; */
				while (strtok(NULL, " ,\t\n") != NULL) (*numCoords)++;
				break; /* this makes read from 1st object */
			}
		}
		rewind(infile);
		
		printf("File %s numObjs   = %d\n", filename, *numObjs);
		printf("File %s numCoords = %d\n", filename, *numCoords);
		

		// allocate space for objects[][] and read all objects 
		len = (*numObjs) * (*numCoords);
		objects = (float**)malloc((*numObjs) * sizeof(float*));
		assert(objects != NULL);
		objects[0] = (float*)malloc(len * sizeof(float));
		assert(objects[0] != NULL);
		for (i = 1; i < (*numObjs); i++)
		{
			objects[i] = objects[i - 1] + (*numCoords);
		}
			
		i = 0;
		// read all objects 
		while (fgets(line, lineLen, infile) != NULL) 
		{
			if (strtok(line, " \t\n") == NULL) continue;
			for (j = 0; j < (*numCoords); j++)
				objects[i][j] = atof(strtok(NULL, " ,\t\n"));
			i++;
		}

		fclose(infile);
		free(line);
	}

	return objects;
}

// file_write() 
int writeToFile(char      *filename,     // input file name 
	int        numClusters,  // no. clusters 
	int        numObjs,      // no. data objects 
	int        numCoords,    // no. coordinates (local) 
	float    **clusters,     // [numClusters][numCoords] centers 
	int       *membership)   // [numObjs] 
{
	FILE *fptr;
	int   i, j;
	char  outFileName[1024];

	// output: the coordinates of the cluster centres 
	sprintf(outFileName, "%s.cluster_centres", filename);
	printf("Writing coordinates of K=%d cluster centers to file \"%s\"\n",
		numClusters, outFileName);
	fptr = fopen(outFileName, "w");
	for (i = 0; i<numClusters; i++) {
		fprintf(fptr, "%d ", i);
		for (j = 0; j<numCoords; j++)
			fprintf(fptr, "%f ", clusters[i][j]);
		fprintf(fptr, "\n");
	}
	fclose(fptr);

	// output: the closest cluster centre to each of the data points 
	sprintf(outFileName, "%s.membership", filename);
	printf("Writing membership of N=%d data objects to file \"%s\"\n",
		numObjs, outFileName);
	fptr = fopen(outFileName, "w");
	for (i = 0; i<numObjs; i++)
		fprintf(fptr, "%d %d\n", i, membership[i]);
	fclose(fptr);

	return 1;
}
static inline int nextPowerOfTwo(int n) 
{
	n--;

	n = n >> 1 | n;
	n = n >> 2 | n;
	n = n >> 4 | n;
	n = n >> 8 | n;
	n = n >> 16 | n;
	//  n = n >> 32 | n;    //  For 64-bit ints

	return ++n;
}


// square of Euclidian distance between two multi-dimensional points
__host__ __device__ inline static
float eucledianDistanceSquared(int    numCoords,
	int    numObjs,
	int    numClusters,
	float *objects,     // [numCoords][numObjs]
	float *clusters,    // [numCoords][numClusters]
	int    objectId,
	int    clusterId)
{
	int i;
	float ans = 0.0;

	for (i = 0; i < numCoords; i++) 
	{
		//objects objects[numObjs * i + objectId] can be thought of as objects[i][objectId], so the ith coordinate for that objectId
		ans += (objects[numObjs * i + objectId] - clusters[numClusters * i + clusterId]) * (objects[numObjs * i + objectId] - clusters[numClusters * i + clusterId]);		
	}

	return(ans);
}

__global__ static
void findNearestCluster(int numCoords,
	int numObjs,
	int numClusters,
	float *objects,           //  [numCoords][numObjs]
	float *deviceClusters,    //  [numCoords][numClusters]
	int *membership,          //  [numObjs]
	int *intermediates)
{
	extern __shared__ char sharedMemory[];

	//  The type chosen for membershipChanged must be large enough to support
	//  reductions. There are blockDim.x elements, one for each thread in the
	//  block. See numThreadsPerClusterBlock in cuda_kmeans().
	unsigned char *membershipChanged = (unsigned char *)sharedMemory;

	float *clusters = deviceClusters;


	membershipChanged[threadIdx.x] = 0;


	int objectId = blockDim.x * blockIdx.x + threadIdx.x;

	if (objectId < numObjs) 
	{
		int   index, i;
		float dist, min_dist;

		// find the cluster id that has min distance to object 
		index = 0;
		min_dist = eucledianDistanceSquared(numCoords, numObjs, numClusters, objects, clusters, objectId, 0);

		for (i = 1; i < numClusters; i++) 
		{
			dist = eucledianDistanceSquared(numCoords, numObjs, numClusters, objects, clusters, objectId, i);
			// do not need square root 
			if (dist < min_dist) 
			{ // find the min and its array index 
				min_dist = dist;
				index = i;
			}
		}

		if (membership[objectId] != index) 
		{
			membershipChanged[threadIdx.x] = 1;
		}

		/* assign the membership to object objectId */
		membership[objectId] = index;

		__syncthreads();    //  For membershipChanged[]

							//  blockDim.x *must* be a power of two!
		for (unsigned int s = blockDim.x / 2; s > 0; s >>= 1) 
		{
			if (threadIdx.x < s) 
			{
				membershipChanged[threadIdx.x] += membershipChanged[threadIdx.x + s];
			}
			__syncthreads();
		}

		if (threadIdx.x == 0) 
		{
			intermediates[blockIdx.x] = membershipChanged[0];
		}
	}
}
///Performs array reduction on the itermediates
__global__ static
void computeDelta(int *deviceIntermediates,
	int numIntermediates,    //  The actual number of intermediates
	int numIntermediates2)   //  The next power of two
{
	//  The number of elements in this array should be equal to
	//  numIntermediates2, the number of threads launched. It *must* be a power
	//  of two!
	extern __shared__ unsigned int intermediates[];

	//  Copy global intermediate values into shared memory.
	intermediates[threadIdx.x] = (threadIdx.x < numIntermediates) ? deviceIntermediates[threadIdx.x] : 0;

	__syncthreads();

	//  numIntermediates2 *must* be a power of two!
	for (unsigned int s = numIntermediates2 / 2; s > 0; s >>= 1) 
	{
		if (threadIdx.x < s) 
		{
			intermediates[threadIdx.x] += intermediates[threadIdx.x + s];
		}
		__syncthreads();
	}

	if (threadIdx.x == 0) 
	{
		deviceIntermediates[0] = intermediates[0];
	}
}

//  Variable explanation
//
//  objects         [numObjs][numCoords]
//  clusters        [numClusters][numCoords]
//  dimObjects      [numCoords][numObjs]
//  dimClusters     [numCoords][numClusters]
//  newClusters     [numCoords][numClusters]
//  deviceObjects   [numCoords][numObjs]
//  deviceClusters  [numCoords][numClusters]
//
// return an array of cluster centers of size [numClusters][numCoords]      

float** CudaKmeans(float **objects,      // in: [numObjs][numCoords] 
	int     numCoords,    // num of features for example RGB color is 3 coordinates
	int     numObjs,      // num of objects 
	int     numClusters,  // num of clusters 
	float   threshold,    // objects change membership 
	int    *membership,   // output [numObjs] 
	int    *loop_iterations)
{
	int      i, j, index, loop = 0;
	int     *newClusterSize; // [numClusters]: no. objects assigned in each new cluster 
							 
	float    delta;          // % of objects change their clusters 
	float  **dimObjects;
	float  **clusters;       // out: [numClusters][numCoords] 
	float  **dimClusters;
	float  **newClusters;    // [numCoords][numClusters] 

	float *deviceObjects;
	float *deviceClusters;
	int *deviceMembership;
	int *deviceIntermediates;

	//  Copy objects given in [numObjs][numCoords] layout to new
	//  [numCoords][numObjs] layout
	malloc2D(dimObjects, numCoords, numObjs, float);
	for (i = 0; i < numCoords; i++) 
	{
		for (j = 0; j < numObjs; j++) 
		{
			dimObjects[i][j] = objects[j][i];
		}
	}

	// pick first numClusters elements of objects[] as initial cluster centers
	malloc2D(dimClusters, numCoords, numClusters, float);
	for (i = 0; i < numCoords; i++) 
	{
		for (j = 0; j < numClusters; j++) 
		{
			dimClusters[i][j] = dimObjects[i][j];
		}
	}

	// initialize membership[] 
	for (i = 0; i < numObjs; i++) 
	{
		membership[i] = -1;
	}

	// need to initialize newClusterSize and newClusters[0] to all 0 
	newClusterSize = (int*)calloc(numClusters, sizeof(int));
	assert(newClusterSize != NULL);

	malloc2D(newClusters, numCoords, numClusters, float);
	memset(newClusters[0], 0, numCoords * numClusters * sizeof(float));

	//  To support reduction, numThreadsPerClusterBlock *must* be a power of
	//  two, and it must be no larger than the number of bits that will
	//  fit into an unsigned char, the type used to keep track of membership
	//  changes in the kernel.
	const unsigned int numThreadsPerClusterBlock = 128;
	const unsigned int numClusterBlocks = (numObjs + numThreadsPerClusterBlock - 1) / numThreadsPerClusterBlock;
		

	const unsigned int clusterBlockSharedDataSize = numThreadsPerClusterBlock * sizeof(unsigned char);
		
	const unsigned int numReductionThreads = nextPowerOfTwo(numClusterBlocks);
		
	const unsigned int reductionBlockSharedDataSize = numReductionThreads * sizeof(unsigned int);
	

	checkCuda(cudaMalloc(&deviceObjects, numObjs * numCoords * sizeof(float)), "allocating device objects");
	checkCuda(cudaMalloc(&deviceClusters, numClusters * numCoords * sizeof(float)), "allocating device clusters");
	checkCuda(cudaMalloc(&deviceMembership, numObjs * sizeof(int)), "allocating device memship");
	checkCuda(cudaMalloc(&deviceIntermediates, numReductionThreads * sizeof(unsigned int)), "allocating device intermediates");

	checkCuda(cudaMemcpy(deviceObjects, dimObjects[0],	numObjs*numCoords*sizeof(float), cudaMemcpyHostToDevice), "memcpy dimObjects[0] -> device objects");
	checkCuda(cudaMemcpy(deviceMembership, membership,	numObjs*sizeof(int), cudaMemcpyHostToDevice), "memcpy membership -> device membership");

	do 
	{
		checkCuda(cudaMemcpy(deviceClusters, dimClusters[0], numClusters*numCoords*sizeof(float), cudaMemcpyHostToDevice), "memcpy dimClusters[0] -> device clusters");
		printf("(for findNearestCluster) numClusterBlocks: %d, numThreadsPerClusterBlock: %d, clusterBlockSharedDataSize: %d \n", numClusterBlocks, numThreadsPerClusterBlock, clusterBlockSharedDataSize);
		findNearestCluster
			<< < numClusterBlocks, numThreadsPerClusterBlock, clusterBlockSharedDataSize >> >(numCoords, numObjs, numClusters, deviceObjects, deviceClusters, deviceMembership, deviceIntermediates);

		cudaDeviceSynchronize(); 
		checkLastCudaError("findNearestCluster");
		printf("(for computeDelta) numReductionThreads: %d, reductionBlockSharedDataSize: %d \n", numReductionThreads, reductionBlockSharedDataSize);
		const int numOfThreadsPerBlock = (numReductionThreads > 1024 ? 1024 : numReductionThreads); // maximum amount of threads per block is 1024
		computeDelta << < 1, numOfThreadsPerBlock, reductionBlockSharedDataSize >> >(deviceIntermediates, numClusterBlocks, numReductionThreads);
		cudaDeviceSynchronize(); 
		checkLastCudaError("computeDelta");

		int d;
		checkCuda(cudaMemcpy(&d, deviceIntermediates, sizeof(int), cudaMemcpyDeviceToHost), "memcpy deviceIntermediates -> &d");
		delta = (float)d;

		checkCuda(cudaMemcpy(membership, deviceMembership, numObjs*sizeof(int), cudaMemcpyDeviceToHost), "memcpy deviceMembership -> membership");

		for (i = 0; i < numObjs; i++) 
		{
			// find the array index of nestest cluster center 
			index = membership[i];

			// update new cluster centers : sum of objects located within
			newClusterSize[index]++;
			for (j = 0; j < numCoords; j++)
				newClusters[j][index] += objects[i][j];
		}

		// average the sum and replace old cluster centers with newClusters 
		for (i = 0; i < numClusters; i++) 
		{
			for (j = 0; j < numCoords; j++) 
			{
				if (newClusterSize[i] > 0) 
				{
					dimClusters[j][i] = newClusters[j][i] / newClusterSize[i];
				}					
				newClusters[j][i] = 0.0;   // set back to 0 
			}
			newClusterSize[i] = 0;   // set back to 0 
		}

		delta /= numObjs;
	} while (delta > threshold && loop++ < 500); // 500 is max number of iterations

	*loop_iterations = loop + 1;

	// allocate a 2D space for returning variable clusters[] (coordinatesof cluster centers) 
	
	malloc2D(clusters, numClusters, numCoords, float);
	for (i = 0; i < numClusters; i++) 
	{
		for (j = 0; j < numCoords; j++) 
		{
			clusters[i][j] = dimClusters[j][i];
		}
	}

	checkCuda(cudaFree(deviceObjects), "free deviceObjects");
	checkCuda(cudaFree(deviceClusters), "free deviceClusters");
	checkCuda(cudaFree(deviceMembership), "free deviceMembership");
	checkCuda(cudaFree(deviceIntermediates), "free deviceIntermediates");

	free(dimObjects[0]);
	free(dimObjects);
	free(dimClusters[0]);
	free(dimClusters);
	free(newClusters[0]);
	free(newClusters);
	free(newClusterSize);

	return clusters;
}
int main(int argc, char **argv)
{
	int     isBinaryFile, is_output_timing;

	int     numClusters, numCoords, numObjs;
	int    *membership;    // [numObjs] 
	char   *filename;
	float **objects;       // [numObjs][numCoords] data objects 
	float **clusters;      // [numClusters][numCoords] cluster center 
	float   threshold;
	double  timing, io_timing, clustering_timing;
	int     loop_iterations;
	threshold = 0.001;
	numClusters = 0;
	isBinaryFile = 0;
	is_output_timing = 1;
	
	if (argc < 3)
	{
		printf("argv[1] is filename and argv[2] is numOfClusters");
		return -1;
	}
	filename = argv[1];
	numClusters = atoi(argv[2]);
	if (is_output_timing) io_timing = clock();

	// read data points from file 
	objects = readFromFile(isBinaryFile, filename, &numObjs, &numCoords);
	if (objects == NULL) exit(1);

	if (is_output_timing) 
	{
		timing = clock();
		io_timing = timing - io_timing;
		clustering_timing = timing;
	}

	// start the timer for the core computation
	// membership: the cluster id for each data object 
	membership = (int*)malloc(numObjs * sizeof(int));
	assert(membership != NULL);

	clusters = CudaKmeans(objects, numCoords, numObjs, numClusters, threshold,
		membership, &loop_iterations);

	free(objects[0]);
	free(objects);

	if (is_output_timing) 
	{
		timing = clock();
		clustering_timing = (timing - clustering_timing) / CLOCKS_PER_SEC;
	}

	// output: the coordinates of the cluster centres
	writeToFile(filename, numClusters, numObjs, numCoords, clusters,
		membership);

	free(membership);
	free(clusters[0]);
	free(clusters);

	// output performance number..You can also see kernel execution time in Visual Studio 2017
	if (is_output_timing) 
	{
		io_timing += clock() - timing;
		printf("\nPerforming **** Regular Kmeans (CUDA version) ****\n");

		printf("Input file:     %s\n", filename);
		printf("numObjs       = %d\n", numObjs);
		printf("numCoords     = %d\n", numCoords);
		printf("numClusters   = %d\n", numClusters);
		printf("threshold     = %.4f\n", threshold);

		printf("Loop iterations    = %d\n", loop_iterations);

		printf("I/O time           = %10.4f sec\n", io_timing);
		printf("Computation timing = %10.4f sec\n", clustering_timing);
	}

    return 0;
}

