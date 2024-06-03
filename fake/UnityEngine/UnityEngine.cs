using System;

namespace UnityEngine;

public class Debug {
  public static void Log(string message) {
    Console.WriteLine(message);
  }

  public static void Assert(bool condition, string message) {}
}

public abstract class MonoBehaviour : Component {
  public static void DontDestroyOnLoad(Object target) {}
}

public class Vector3 {

  public static Vector3 operator-(Vector3 unary) {
    throw new NotImplementedException();
  }

  public static Vector3 operator-(Vector3 a, Vector3 b) {
    throw new NotImplementedException();
  }

  public static Vector3 operator*(Vector3 a, float scalar) {
    throw new NotImplementedException();
  }
}

public class Quaternion {
  public static Quaternion identity;
}

public class Transform {
  public Vector3 position;
  public Vector3 forward;
  public Vector3 localPosition;
  public Quaternion localRotation;

  public Transform parent;

  public void Rotate(float x, float y, float z) {
    throw new NotImplementedException();
  }
}

public class AudioSource : Component {
  public bool playOnAwake;
  public bool loop;
  public float volume;
  public float spatialBlend;
  public AudioRolloffMode rolloffMode;
  public float dopplerLevel;
}

public class AudioClip {}

public enum AudioRolloffMode {
  Logarithmic, 
}


public class GameObject : Object {
  public string name;
  public T AddComponent<T>() where T : Component {
    throw new NotImplementedException();
  }
}

public class Component : Object {
  public GameObject gameObject;
  public Transform transform;

  public Component GetComponent(Type type) {
    throw new NotImplementedException();
  }
}

public class Light : Component {}

public class ParticleSystem : Component {
}

public class Object {
  public static T Instantiate<T>(T original) where T : Object {
    throw new NotImplementedException();
  }

  public static void Destroy(Object obj) {
    throw new NotImplementedException();
  }
}