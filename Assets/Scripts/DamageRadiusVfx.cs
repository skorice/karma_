using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DamageRadiusVfx : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField]
    private string sortingLayerName = "GroundVFX";

    [SerializeField]
    private int baseSortingOrder = 0;

    [Header("Persistent Aura")]
    [SerializeField, Range(0f, 0.2f)]
    private float pulseAmount = 0.018f;

    [SerializeField, Min(0f)]
    private float pulseSpeed = 1.4f;

    private readonly List<Object> runtimeAssets = new();

    private Transform persistentRoot;

    private ParticleSystem pulseRing;
    private ParticleSystem pulseFlash;
    private ParticleSystem pulseSmoke;
    private ParticleSystem pulseSparks;

    private bool initialized;

    private void Update()
    {
        if (persistentRoot == null)
            return;

        // Контейнер всегда остаётся строго в центре объекта эффекта.
        persistentRoot.localPosition = Vector3.zero;
        persistentRoot.localRotation = Quaternion.identity;

        float pulse =
            1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        persistentRoot.localScale = new Vector3(pulse, pulse, 1f);
    }

    /// <summary>
    /// Вызывается боевой системой в момент нанесения урона.
    /// </summary>
    public void PlayAttackPulse()
    {
        if (!initialized)
            return;

        EmitCenteredParticle(pulseRing);
        EmitCenteredParticle(pulseFlash);

        pulseSmoke.Emit(18);
        pulseSparks.Emit(24);
    }

    public void BuildEffect(float radius)
    {
        if (initialized)
            return;

        Shader shader = FindParticleShader();

        if (shader == null)
        {
            Debug.LogError(
                "DamageRadiusVfx: compatible URP particle shader was not found.",
                this);

            enabled = false;
            return;
        }

        Material areaMaterial = CreateMaterial(
            shader,
            CreateTexture(TextureKind.Area, 128),
            "Aura Area Material");

        Material ringMaterial = CreateMaterial(
            shader,
            CreateTexture(TextureKind.Ring, 128),
            "Aura Ring Material");

        Material smokeMaterial = CreateMaterial(
            shader,
            CreateTexture(TextureKind.Smoke, 128),
            "Aura Smoke Material");

        Material moteMaterial = CreateMaterial(
            shader,
            CreateTexture(TextureKind.Mote, 64),
            "Aura Mote Material");

        persistentRoot = new GameObject("Persistent Aura").transform;
        persistentRoot.SetParent(transform, false);
        persistentRoot.localPosition = Vector3.zero;
        persistentRoot.localRotation = Quaternion.identity;
        persistentRoot.localScale = Vector3.one;

        CreateStaticParticle(
            "Soft Area",
            persistentRoot,
            areaMaterial,
            radius * 2f,
            new Color(0.72f, 0.75f, 0.78f, 0.19f),
            baseSortingOrder);

        CreateStaticParticle(
            "Outer Boundary",
            persistentRoot,
            ringMaterial,
            radius * 2f,
            new Color(0.92f, 0.94f, 0.96f, 0.42f),
            baseSortingOrder + 1);

        CreateAmbientMotes(moteMaterial, radius);

        pulseRing = CreatePulseRing(ringMaterial, radius);
        pulseFlash = CreatePulseFlash(areaMaterial, radius);
        pulseSmoke = CreatePulseSmoke(smokeMaterial, radius);
        pulseSparks = CreatePulseSparks(moteMaterial, radius);

        initialized = true;
    }

    private ParticleSystem CreateSystem(
        string systemName,
        Transform parent,
        Material material,
        int sortingOrder)
    {
        GameObject systemObject = new(systemName);

        Transform systemTransform = systemObject.transform;
        systemTransform.SetParent(parent, false);
        systemTransform.localPosition = Vector3.zero;
        systemTransform.localRotation = Quaternion.identity;
        systemTransform.localScale = Vector3.one;

        ParticleSystem system =
            systemObject.AddComponent<ParticleSystem>();

        // Новый Particle System может начать воспроизведение сразу после
        // AddComponent. Его нужно остановить перед изменением duration.
        system.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystemRenderer renderer =
            systemObject.GetComponent<ParticleSystemRenderer>();

        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.View;
        renderer.material = material;
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = sortingOrder;

        ParticleSystem.MainModule main = system.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 1f;
        main.startLifetime = 1f;
        main.startSpeed = 0f;
        main.startSize = 1f;
        main.startRotation = new ParticleSystem.MinMaxCurve(
            0f,
            Mathf.PI * 2f);
        main.startColor = Color.white;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        main.maxParticles = 64;

        ParticleSystem.EmissionModule emission = system.emission;
        emission.enabled = false;

        // Shape отключён по умолчанию. Системы, которым он нужен,
        // включают и настраивают его отдельно.
        ParticleSystem.ShapeModule shape = system.shape;
        shape.enabled = false;

        return system;
    }

    private void CreateStaticParticle(
        string systemName,
        Transform parent,
        Material material,
        float diameter,
        Color color,
        int sortingOrder)
    {
        ParticleSystem system = CreateSystem(
            systemName,
            parent,
            material,
            sortingOrder);

        ParticleSystem.MainModule main = system.main;
        main.startLifetime = 100000f;
        main.startSize = diameter;
        main.startColor = color;
        main.maxParticles = 1;

        ParticleSystem.EmitParams emitParams = new()
        {
            position = Vector3.zero,
            velocity = Vector3.zero,
            startSize = diameter,
            startColor = color,
            startLifetime = 100000f
        };

        system.Emit(emitParams, 1);
    }

    private void CreateAmbientMotes(Material material, float radius)
    {
        ParticleSystem system = CreateSystem(
            "Ambient Motes",
            persistentRoot,
            material,
            baseSortingOrder + 2);

        ParticleSystem.MainModule main = system.main;
        main.loop = true;
        main.duration = 2f;
        main.startLifetime =
            new ParticleSystem.MinMaxCurve(2.6f, 4.2f);
        main.startSpeed =
            new ParticleSystem.MinMaxCurve(0.05f, 0.18f);
        main.startSize =
            new ParticleSystem.MinMaxCurve(0.12f, 0.35f);
        main.startColor =
            new Color(0.88f, 0.9f, 0.92f, 0.28f);
        main.maxParticles = 32;

        ParticleSystem.EmissionModule emission = system.emission;
        emission.enabled = true;
        emission.rateOverTime = 4f;

        ParticleSystem.ShapeModule shape = system.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius * 0.88f;
        shape.radiusThickness = 1f;
        shape.randomDirectionAmount = 1f;

        ParticleSystem.ColorOverLifetimeModule color =
            system.colorOverLifetime;

        color.enabled = true;
        color.color = CreateFadeGradient(0.28f);

        ParticleSystem.NoiseModule noise = system.noise;
        noise.enabled = true;
        noise.separateAxes = true;
        noise.strengthX = 0.2f;
        noise.strengthY = 0.2f;
        noise.strengthZ = 0f;
        noise.frequency = 0.35f;
        noise.scrollSpeed = 0.2f;

        system.Play();
    }

    private ParticleSystem CreatePulseRing(Material material, float radius)
    {
        ParticleSystem system = CreateSystem(
            "Attack Ring",
            transform,
            material,
            baseSortingOrder + 4);

        ParticleSystem.MainModule main = system.main;
        main.startLifetime = 0.55f;
        main.startSize = radius * 2f;
        main.startColor = new Color(1f, 1f, 1f, 0.9f);
        main.maxParticles = 2;

        ParticleSystem.SizeOverLifetimeModule size =
            system.sizeOverLifetime;

        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.08f),
                new Keyframe(0.18f, 0.72f),
                new Keyframe(1f, 1.12f)));

        ParticleSystem.ColorOverLifetimeModule color =
            system.colorOverLifetime;

        color.enabled = true;
        color.color = CreateFadeGradient(0.9f);

        return system;
    }

    private ParticleSystem CreatePulseFlash(Material material, float radius)
    {
        ParticleSystem system = CreateSystem(
            "Attack Flash",
            transform,
            material,
            baseSortingOrder + 3);

        ParticleSystem.MainModule main = system.main;
        main.startLifetime = 0.28f;
        main.startSize = radius * 2f;
        main.startColor =
            new Color(0.95f, 0.97f, 1f, 0.34f);
        main.maxParticles = 2;

        ParticleSystem.SizeOverLifetimeModule size =
            system.sizeOverLifetime;

        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.55f),
                new Keyframe(0.35f, 1f),
                new Keyframe(1f, 1.04f)));

        ParticleSystem.ColorOverLifetimeModule color =
            system.colorOverLifetime;

        color.enabled = true;
        color.color = CreateFadeGradient(0.34f);

        return system;
    }

    private ParticleSystem CreatePulseSmoke(Material material, float radius)
    {
        ParticleSystem system = CreateSystem(
            "Attack Smoke",
            transform,
            material,
            baseSortingOrder + 2);

        ParticleSystem.MainModule main = system.main;
        main.startLifetime =
            new ParticleSystem.MinMaxCurve(0.5f, 0.9f);
        main.startSpeed =
            new ParticleSystem.MinMaxCurve(0.15f, 0.45f);
        main.startSize =
            new ParticleSystem.MinMaxCurve(0.45f, 1.25f);
        main.startRotation =
            new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor =
            new Color(0.74f, 0.77f, 0.8f, 0.3f);
        main.maxParticles = 32;

        ParticleSystem.ShapeModule shape = system.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius * 0.82f;
        shape.radiusThickness = 1f;
        shape.randomDirectionAmount = 1f;

        ParticleSystem.SizeOverLifetimeModule size =
            system.sizeOverLifetime;

        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(
            1f,
            AnimationCurve.EaseInOut(
                0f,
                0.45f,
                1f,
                1.2f));

        ParticleSystem.ColorOverLifetimeModule color =
            system.colorOverLifetime;

        color.enabled = true;
        color.color = CreateFadeGradient(0.3f);

        ParticleSystem.NoiseModule noise = system.noise;
        noise.enabled = true;
        noise.separateAxes = true;
        noise.strengthX = 0.45f;
        noise.strengthY = 0.45f;
        noise.strengthZ = 0f;
        noise.frequency = 0.55f;

        return system;
    }

    private ParticleSystem CreatePulseSparks(Material material, float radius)
    {
        ParticleSystem system = CreateSystem(
            "Attack Sparks",
            transform,
            material,
            baseSortingOrder + 5);

        ParticleSystem.MainModule main = system.main;
        main.startLifetime =
            new ParticleSystem.MinMaxCurve(0.22f, 0.48f);
        main.startSpeed =
            new ParticleSystem.MinMaxCurve(1.8f, 4.2f);
        main.startSize =
            new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
        main.startColor =
            new Color(1f, 1f, 1f, 0.82f);
        main.maxParticles = 32;

        ParticleSystem.ShapeModule shape = system.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius * 0.22f;
        shape.radiusThickness = 1f;
        shape.randomDirectionAmount = 1f;

        ParticleSystem.ColorOverLifetimeModule color =
            system.colorOverLifetime;

        color.enabled = true;
        color.color = CreateFadeGradient(0.82f);

        ParticleSystem.SizeOverLifetimeModule size =
            system.sizeOverLifetime;

        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(
            1f,
            AnimationCurve.Linear(
                0f,
                1f,
                1f,
                0.15f));

        return system;
    }

    private static void EmitCenteredParticle(
        ParticleSystem system)
    {
        if (system == null)
            return;

        ParticleSystem.EmitParams emitParams = new()
        {
            position = Vector3.zero,
            velocity = Vector3.zero
        };

        system.Emit(emitParams, 1);
    }

    private Material CreateMaterial(
        Shader shader,
        Texture2D texture,
        string materialName)
    {
        Material material = new(shader)
        {
            name = materialName,
            hideFlags = HideFlags.DontSave
        };

        if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", texture);

        if (material.HasProperty("_MainTex"))
            material.SetTexture("_MainTex", texture);

        runtimeAssets.Add(material);

        return material;
    }

    private Texture2D CreateTexture(
        TextureKind kind,
        int resolution)
    {
        Texture2D texture = new(
            resolution,
            resolution,
            TextureFormat.RGBA32,
            false)
        {
            name = $"Generated {kind} Texture",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.DontSave
        };

        Color[] pixels = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float nx =
                    (x + 0.5f) / resolution * 2f - 1f;

                float ny =
                    (y + 0.5f) / resolution * 2f - 1f;

                float distance =
                    Mathf.Sqrt(nx * nx + ny * ny);

                float alpha = kind switch
                {
                    TextureKind.Area =>
                        1f - SmoothStep(0.68f, 1f, distance),

                    TextureKind.Ring =>
                        Mathf.Exp(
                            -Mathf.Pow(
                                (distance - 0.9f) / 0.045f,
                                2f)),

                    TextureKind.Smoke =>
                        1f - SmoothStep(0.25f, 1f, distance),

                    TextureKind.Mote =>
                        Mathf.Exp(-distance * distance * 7f),

                    _ => 0f
                };

                pixels[y * resolution + x] =
                    new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);

        runtimeAssets.Add(texture);

        return texture;
    }

    private static Gradient CreateFadeGradient(
        float maximumAlpha)
    {
        Gradient gradient = new();

        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(
                    new Color(0.76f, 0.8f, 0.84f),
                    1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(maximumAlpha, 0.12f),
                new GradientAlphaKey(
                    maximumAlpha * 0.65f,
                    0.62f),
                new GradientAlphaKey(0f, 1f)
            });

        return gradient;
    }

    private static float SmoothStep(
        float from,
        float to,
        float value)
    {
        float t = Mathf.Clamp01(
            (value - from) / (to - from));

        return t * t * (3f - 2f * t);
    }

    private static Shader FindParticleShader()
    {
        return Shader.Find(
                   "Universal Render Pipeline/2D/Sprite-Unlit-Default")
               ?? Shader.Find(
                   "Universal Render Pipeline/Particles/Unlit")
               ?? Shader.Find("Sprites/Default");
    }

    private void OnDestroy()
    {
        foreach (Object asset in runtimeAssets)
        {
            if (asset != null)
                Destroy(asset);
        }

        runtimeAssets.Clear();
    }

    private enum TextureKind
    {
        Area,
        Ring,
        Smoke,
        Mote
    }
}