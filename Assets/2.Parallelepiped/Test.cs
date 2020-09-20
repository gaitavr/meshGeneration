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
            //yield return new WaitForSeconds(5);
            var startIndex = 0;
            for (int y = 0; y < _ySize; y++)
            {
                for (int x = 0; x <= _xSize; x++)
                {
                    CreateSphere(new Vector3(x, y, 0));
                    yield return new WaitForSeconds(delay);
                }

                for (int z = 1; z <= _zSize; z++)
                {
                    CreateSphere(new Vector3(_xSize, y, z));
                    yield return new WaitForSeconds(delay);
                }

                for (int x = _xSize - 1; x >= 0; x--)
                {
                    CreateSphere(new Vector3(x, y, _zSize));
                    yield return new WaitForSeconds(delay);
                }

                for (int z = _zSize - 1; z > 0; z--)
                {
                    CreateSphere(new Vector3(0, y, z));
                    yield return new WaitForSeconds(delay);
                }
                //yield return AnimateSphere(objects[startIndex]);
                startIndex += (_xSize + _zSize) * 2;
            }
            
            for (int z = 1; z < _zSize; z++)
            {
                for (int x = 1; x < _xSize; x++)
                {
                    CreateSphere(new Vector3(x, _ySize - 1, z), false);
                    yield return new WaitForSeconds(delay);
                }
            }
        
            for (int z = 1; z < _zSize; z++)
            {
                for (int x = 1; x < _xSize; x++)
                {
                    CreateSphere(new Vector3(x, 0, z), false);
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        private GameObject CreateSphere(Vector3 pos, bool changeColor = true)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = pos;
            obj.transform.localScale = Vector3.one * 0.3f;
            objects.Add(obj);
            if(!changeColor)
                obj.GetComponent<Renderer>().material.color = Color.green;
            return obj;
        }

        private IEnumerator AnimateSphere(GameObject obj)
        {
            var t = 1f;
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