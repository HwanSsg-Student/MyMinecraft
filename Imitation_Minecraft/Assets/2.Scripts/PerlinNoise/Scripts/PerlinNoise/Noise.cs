using UnityEngine;

/// <summary>
/// mapWidth    : 넓이
/// mapHeight   : 높이
/// seed        : 랜덤 수
/// octaves     : 그려지는 갯수?
/// persistance : 진폭? 높낮이?
/// lacunarity  : 진동수
/// offset      : 차감 계산?
/// 복사본이 생기지 않도록 static으로 정의함
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
        float amplitude = 1f;       // 진폭
        float frequency;            // 진동수

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) + settings.offset.y + sampleCenter.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;      //정규화를 위한 최고 높이
        float minLocalNoiseHeight = float.MaxValue;      //정규화를 위한 최저 높이
        float halfHeight = mapHeight / 2f;
        float halfWidth = mapWidth / 2f;
        

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1f;       // 진폭
                frequency = 1f;       // 진동수
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
                    else //36부터 
                    {
                        boundaryX = boundaryCount % 2 == 0 ? -1 : 16;
                        boundaryY = temp++ / 2;
                    }
                    
                    boundaryCount++;
                    for (int i = 0; i < settings.octaves; i++)       //octave 생성
                    {
                        float sampleX = (boundaryX - halfWidth + octavesOffsets[i].x) / settings.scale * frequency;
                        float sampleY = (boundaryY - halfHeight + octavesOffsets[i].y) / settings.scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //-1 과 1사이의 값
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
                    }
                }
                else
                {
                    for (int i = 0; i < settings.octaves; i++)       //octave 생성
                    {
                        float sampleX = (x - halfWidth + octavesOffsets[i].x) / settings.scale * frequency;       //정수값은 모두 같은 결과를 도출하기 때문에 실수로 나누어야함.
                        float sampleY = (y - halfHeight + octavesOffsets[i].y) / settings.scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //-1 과 1사이의 값
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
            for (int y = 0; y < mapHeight; y++)     //정규화
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