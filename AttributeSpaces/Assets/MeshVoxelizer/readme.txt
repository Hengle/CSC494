-Basic usage: 
    -Open "Window -> Mesh Voxelizer" window
    -Select source GameObject from currently opened scene
    -Adjust settings and voxelize the mesh
    -Voxelized result will be automatically saved as a prefab in "Assets/MeshVoxlizer/Temp" folder

-Property:
    -Generation Type: 
        -Single Mesh: Voxelize the original mesh and replace it with the generated mesh.
        -Separate Voxels: Generate a group of individual voxel game objects instead of voxelize the original mesh.
    -Voxel Size Type: 
        -Subdivision: Set voxel size by subdivision level. 
        -Absolute Size: Set voxel size by absolute voxel size.
    -Precision: 
        Precision level of voxelization, higher precision level means more sampling points will be taken from the original mesh, the voxelization result will be more accurate.
        Note that higher precision requires longer voxelization time.
    -UV Conversion Type: 
        -None: Generated mesh will not have any UV.
        -Source Mesh: Convert UVs from the original mesh. 
        -Voxel Mesh: Individual voxel's UV will be kept.
    -Convert Bone Weights (SkinnedMeshRenderer only): 
        Convert bone weight from the original mesh.
    -Backface Culling (for Generation Type = Single Mesh): 
        Cull backface.
    -Optimization (for Generation Type = Single Mesh): 
        Optimize voxelization result.
    -Fill Center Space (for Generation Type = Separate Voxels)
        Fill model's center space with voxels. Try different axis if the result is incorrect.
    -Center Material (for Generation Type = Separate Voxels):
        Material for center voxels.
    -Approximation:
        Approximate voxels around original mesh's edge/corner, make voxelization result more smooth.
        This is useful when voxelizing origanic objects
    -Modify Voxel: 
        Use custom voxel instead of default cube voxel. 
        Enabling this will disable Backface Culling and Optimization.
    -Voxel Mesh: 
        Basic mesh for voxel.
    -Voxel Scale: 
        Scale individual voxel.
