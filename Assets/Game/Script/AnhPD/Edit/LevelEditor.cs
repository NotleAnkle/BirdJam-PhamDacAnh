using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        [SerializeField] private Bird rootBird;

        [SerializeField] protected CusorBird cusorBird;
        [SerializeField] private GameObject selectIcon;

        [SerializeField] private COLOR color;

        enum EditorState
        {

        }
        public bool isSpawnSleepyBird = true, isSelectRoot = false;
#if UNITY_EDITOR
        private void Update()
        {
            if (!gameObject.activeSelf) return;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                isSpawnSleepyBird = true;
                isSelectRoot = false;

                cusorBird.gameObject.SetActive(isSpawnSleepyBird);
                selectIcon.gameObject.SetActive(isSelectRoot);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                isSelectRoot = true;
                isSpawnSleepyBird = false;

                selectIcon.gameObject.SetActive(isSelectRoot);
                cusorBird.gameObject.SetActive(isSpawnSleepyBird);
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                LevelManager.Ins.isAllowOnMouseDown = !LevelManager.Ins.isAllowOnMouseDown;
                isSelectRoot = false;
                isSpawnSleepyBird = false;

                selectIcon.gameObject.SetActive(isSelectRoot);
                cusorBird.gameObject.SetActive(isSpawnSleepyBird);
            }

            if( Input.GetKeyDown(KeyCode.F))
            {
                color = (COLOR)(((int)color + 1) % 9);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if(cusorBird.isReady && isSpawnSleepyBird)
                {
                    Bird bird = SimplePool.Spawn<Bird>(PoolType.Bird);
                    bird.InitColor(color);
                    bird.OnFall();
                    bird.Freeze(rootBird);
                    rootBird?.AddFollowingSleepyBird(bird);
                    bird.transform.position = cusorBird.transform.position;
                    bird.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                }

                if(isSelectRoot)
                {
                    rootBird = selectBird();
                }
            }

            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
#endif
        private Bird selectBird()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Bird bird = hit.collider.GetComponent<Bird>();
                if (bird != null)
                {
                    return bird;
                }
            }

            return null;
        }
    }
}

