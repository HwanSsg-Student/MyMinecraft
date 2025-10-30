using UnityEngine;

public static class BlockDataGenerator
{
    public static BlockData GenerateBlockData(HeightMap heightMap, BlockSettings blockSettings, HeightMapSettings heightMapSettings)
    {
        BlockData blockData = new BlockData(blockSettings);
        int width = heightMap.values.GetLength(0);
        int height = blockSettings.MaxHeight;

        System.Random rnd = new System.Random((int)(heightMap.sampleCenter.x) * 100000 + (int)(heightMap.sampleCenter.y));
        int count = rnd.Next(1, 5);


        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int h = 0; h < height + 1; h++)
                {
                    if (h > heightMap.values[x, z])     //공기
                    {
                        blockData.AddBlockData(x, h, z, BlockType.Air);
                    }
                    else
                    {
                        if (h + 1 > heightMap.values[x, z]) //꼭대기
                        {
                            blockData.AddBlockData(x, h, z, BlockType.Grass);
                        }
                        else //나머지
                        {
                            blockData.AddBlockData(x, h, z);
                        }
                    }
                }

            }
        }

        for (int i = 0; i < count; i++)
        {
            int treeHeight = rnd.Next(5, 8);

            int sampleX = (int)(rnd.NextDouble() * 10) + 3;
            int sampleZ = (int)(rnd.NextDouble() * 10) + 3;

            for (int j = 1; j <= treeHeight + 1; j++)
            {
                if (j <= treeHeight)
                {
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Wood);
                }
                if (j == treeHeight)
                {
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                }
                if (j == treeHeight + 1)
                {
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                }
                else if (j >= treeHeight - 2 && j < treeHeight)
                {
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 2, BlockType.Leaf);
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 2, BlockType.Leaf);

                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ, BlockType.Leaf);

                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);

                    blockData.AddBlockData(sampleX - 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);

                    blockData.AddBlockData(sampleX - 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 1, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 2, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 1, BlockType.Leaf);

                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 2, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ - 2, BlockType.Leaf);
                    blockData.AddBlockData(sampleX - 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 2, BlockType.Leaf);
                    blockData.AddBlockData(sampleX + 1, (int)heightMap.values[sampleX, sampleZ] + j, sampleZ + 2, BlockType.Leaf);
                }
            }
        }

        return blockData;
    }
}


public class BlockData
{
    readonly int _width;
    readonly int _height;
    BlockSettings _blockSettings;
    public BlockType[,,] _blockTypes;    //어떤 블럭인지

    public BlockData(BlockSettings blockSettings)
    {
        _blockSettings = blockSettings;
        _width = blockSettings.width;
        _height = blockSettings.MaxHeight;
        _blockTypes = new BlockType[_width, _height + 1, _width];
    }


    public void AddBlockData(int x, int y, int z, BlockType blockType = BlockType.None)
    {        
        if(blockType == BlockType.None)
        {
            for (int i = 0; i < _blockSettings.blockDatas.Length; i++)
            {
                if (y < _blockSettings.blockDatas[i].m_startHeight)
                {
                    break;
                }
                else
                {
                    _blockTypes[x, y, z] = _blockSettings.blockDatas[i].m_blockType;
                }
            }
        }
        else
        {
            try
            {
                _blockTypes[x, y, z] = blockType;
            }
            catch (System.Exception e)
            {
                Debug.Log("x: " + x + " y: " + y + " z: " + z);
                Debug.Log("blockType : " + blockType);
                Debug.Log(e.Message.ToString());
                throw;
            }
        }

    }


}