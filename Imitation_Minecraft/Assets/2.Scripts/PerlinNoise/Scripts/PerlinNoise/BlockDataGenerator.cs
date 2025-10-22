using UnityEngine;

public static class BlockDataGenerator
{
    public static BlockData GenerateBlockData(HeightMap heightMap, BlockSettings blockSettings, HeightMapSettings heightMapSettings, Transform viewer)
    {
        BlockData blockData = new BlockData(blockSettings);
        int width = blockSettings.width;
        int height = blockSettings.MaxHeight;

        System.Random rnd = new System.Random((int)(heightMap.sampleCenter.x) * 100000 + (int)(heightMap.sampleCenter.y));
        int count = rnd.Next(1, 5);
        

        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int h = 0; h < height + 7; h++)
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
    public int width;
    public int height;
    BlockSettings m_blockSettings;
    public BlockType[,,] m_blockTypes;    //어떤 블럭인지
    //public BlockType[][][] m_bringBlockTypeFromJson;
    
    

    public BlockData(BlockSettings blockSettings)
    {
        m_blockSettings = blockSettings;
        width = blockSettings.width;
        height = blockSettings.MaxHeight;
        m_blockTypes = new BlockType[width, height + 7, width];
    }


    public void AddBlockData(int x, int y, int z, BlockType blockType = BlockType.None)
    {        
        if(blockType == BlockType.None)
        {
            for (int i = 0; i < m_blockSettings.blockDatas.Length; i++)
            {
                if (y < m_blockSettings.blockDatas[i].m_startHeight)
                {
                    break;
                }
                else
                {
                    m_blockTypes[x, y, z] = m_blockSettings.blockDatas[i].m_blockType;
                }
            }
        }
        else
        {
            try
            {
                m_blockTypes[x, y, z] = blockType;
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