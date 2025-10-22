using UnityEngine;

/// <summary>
/// mapWidth    : ����
/// mapHeight   : ����
/// seed        : ���� ��
/// octaves     : �׷����� ����?
/// persistance : ����? ������?
/// lacunarity  : ������
/// offset      : ���� ���?
/// ���纻�� ������ �ʵ��� static���� ������
/// </summary>

public static class Noise
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        int boundaryCount = 0;
        int temp = 0;
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octavesOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1f;       // ����
        float frequency;            // ������

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) + settings.offset.y + sampleCenter.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;      //����ȭ�� ���� �ְ� ����
        float minLocalNoiseHeight = float.MaxValue;      //����ȭ�� ���� ���� ����
        float halfHeight = mapHeight / 2f;
        float halfWidth = mapWidth / 2f;
        

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1f;       // ����
                frequency = 1f;       // ������
                float noiseHeight = 0;

                if (x >= 16 || y >= 16)
                {

                    int boundaryX;
                    int boundaryY;

                    if(boundaryCount < 36)
                    {
                        boundaryX = boundaryCount % mapWidth - 1;
                        boundaryY = boundaryCount < 18 ? -1 : 16;
                    }
                    else //36���� 
                    {
                        boundaryX = boundaryCount % 2 == 0 ? -1 : 16;
                        boundaryY = temp++ / 2;
                    }
                    
                    boundaryCount++;
                    for (int i = 0; i < settings.octaves; i++)       //octave ����
                    {
                        float sampleX = (boundaryX - halfWidth + octavesOffsets[i].x) / settings.scale * frequency;
                        float sampleY = (boundaryY - halfHeight + octavesOffsets[i].y) / settings.scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //-1 �� 1������ ��
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
                    }
                }
                else
                {
                    for (int i = 0; i < settings.octaves; i++)       //octave ����
                    {
                        float sampleX = (x - halfWidth + octavesOffsets[i].x) / settings.scale * frequency;       //�������� ��� ���� ����� �����ϱ� ������ �Ǽ��� ���������.
                        float sampleY = (y - halfHeight + octavesOffsets[i].y) / settings.scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //-1 �� 1������ ��
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
                    }
                }
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

                if(settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }

            }
        }
        
        
        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)     //����ȭ
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }
        return noiseMap;
    }

    
}
[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50f;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = 0.6f;
    public float lacunarity = 2f;

    public int seed;
    public Vector2Int offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}