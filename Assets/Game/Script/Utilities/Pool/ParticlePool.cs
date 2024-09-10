using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticlePool
{
    static int DEFAULT_POOL = 10;
    static Dictionary<ParticleSystem, Particle> particlePools = new Dictionary<ParticleSystem, Particle>();

    public static void Preload(ParticleSystem particle, int amount, Transform parent)
    {
        if (!particlePools.ContainsKey(particle) || particlePools[particle] == null)
        {
            particlePools.Add(particle, new Particle(particle, DEFAULT_POOL, parent));
        }
    }

    public static void Play(ParticleSystem particle, Vector3 position, Quaternion rotation)
    {
        if (!particlePools.ContainsKey(particle) || particlePools[particle] == null)
        {
            particlePools.Add(particle, new Particle(particle, DEFAULT_POOL, null));
        }

        particlePools[particle].Play(position, rotation);
    }

    public static void CollectAll()
    {
        foreach (var item in particlePools)
        {
            item.Value.Collect();
        }
    }

    public static void ReleaseAll()
    {
        foreach (var item in particlePools)
        {
            item.Value.Release();
        }
        particlePools.Clear();
    }

    public class Particle
    {
        List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        ParticleSystem particle;
        Transform parent;
        int index;

        public Particle(ParticleSystem particle, int amount, Transform parent)
        {
            this.particle = particle;
            this.parent = parent;

            for (int i = 0; i < amount; i++)
            {
                particleSystems.Add(GameObject.Instantiate(particle, parent));
            }

            index = 0;
        }

        public void Play(Vector3 position, Quaternion rotation)
        {
            ParticleSystem vfx = null;

            index = index + 1 >= particleSystems.Count ? 0 : index + 1;

            if (particleSystems[index].isPlaying)
            {
                vfx = GameObject.Instantiate(particle, parent);
                particleSystems.Insert(index, vfx);
            }

            particleSystems[index].transform.SetPositionAndRotation(position, rotation);
            particleSystems[index].Play();
        }

        public void Collect()
        {
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Stop();
            }
        }

        public void Release()
        {
            while (particleSystems.Count > 0)
            {
                GameObject.Destroy(particleSystems[0]);
                particleSystems.RemoveAt(0);
            }
        }
    }
}