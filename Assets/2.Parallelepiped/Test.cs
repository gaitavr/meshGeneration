using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2.Parallelepiped
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private int _xSize, _ySize, _zSize;

        public float delay;
        
        List<GameObject> objects = new List<GameObject>();
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(5);
            for (int x = 0; x <= _xSize; x++)
            {
                CreateSphere(new Vector3(x, _ySize, 0));
                yield return new WaitForSeconds(delay);
            }

            for (int z = 1; z <= _zSize; z++)
            {
                CreateSphere(new Vector3(_xSize, _ySize, z));
                yield return new WaitForSeconds(delay);
            }

            for (int x = _xSize - 1; x >= 0; x--)
            {
                CreateSphere(new Vector3(x, _ySize, _zSize));
                yield return new WaitForSeconds(delay);
            }

            for (int z = _zSize - 1; z > 0; z--)
            {
                var obj = CreateSphere(new Vector3(0, _ySize, z));
                yield return new WaitForSeconds(delay);
            }
            
            yield return AnimateSphere(objects[0]);
        }

        private GameObject CreateSphere(Vector3 pos)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = pos;
            obj.transform.localScale = Vector3.one * 0.3f;
            objects.Add(obj);
            return obj;
        }

        private IEnumerator AnimateSphere(GameObject obj)
        {
            var t = 2f;
            while (t > 0)
            {
                obj.transform.localScale = Vector3.one * Mathf.Sin(t * 5) * 0.5f;
                t -= Time.deltaTime;
                yield return null;
            }
            obj.transform.localScale = Vector3.one * 0.3f;
        }
    }
}