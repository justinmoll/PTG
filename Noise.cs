using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        // This function generates a 2D array of values between 0 and 1 based off of perlin noise.
        
        // Set the length of the 2D array to be the width and height entered. 
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Create a new random seed.
        System.Random prng = new System.Random(seed);
        
        // create a new array of Vector2 whos length is equal to how many octaves were given.
        Vector2[] octaveOffSets = new Vector2[octaves];
        
        // loop through each octave and give the offset a random seed. 
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffSets[i] = new Vector2(offsetX, offsetY);
        }
        
        // Clamp the scale value.
        if (scale <= 0) {
            scale = 0.00001f;
        }

        // set the max noise height as -bajillion
        // set the current min noise height as +bajillion
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Get half the width and height of the maps as a float so that scaling happens in the center. 
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        
        // loop through the width and height passed through
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++)
            {

                //set the amplitude, frequency and noise height of the noise. 
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                
                // for each octave.
                for (int i = 0; i < octaves; i++) {
                    
                    //Get a sample. Half it to get to center. Divide it by scale * current freq. Offset it by current octave.
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffSets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffSets[i].y;

                    // get the perlin value.
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    
                    // The noise height is brought down by amplitude every single time. Higher amplitude, less effect.
                    noiseHeight += perlinValue * amplitude;

                    //modify amplitude and frequency.
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //clamps the noise height so that you dont just get absurdly bright noise maps of a billion.
                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                //update the noise map with the current noisemap height.
                noiseMap[x, y] = noiseHeight;
            }
        }
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //this clamps the noise map between the highest and lowest possible noise heights. 
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
