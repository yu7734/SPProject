using System.Linq;
using UnityEngine;

enum Direction 
{ 
    forward,
    backward, 
    up, 
    down, 
    left, 
    right
}
public class RaycastTrackingScript : MonoBehaviour
{
    string SearchTag = Tags.Enemy;

    [Tooltip("サーチ範囲(円柱状)")]public float radius = 1f; // 円柱の半径
    [SerializeField,Tooltip("サーチする方向")] Direction direction;
    [Tooltip("サーチ距離")]public float maxDistance = 100f; // サーチ距離
    Vector3 point; // カプセルの中心
    RaycastHit[] raycastHits;
    GameObject[] raycastHitsObjects;
    GameObject target;
    Vector3 _direction;
    bool target_lost=false;

    private void Awake()
    {
        switch (direction)
        {
            case Direction.forward: _direction = transform.forward; break;
            case Direction.backward: _direction = -transform.forward; break;
            case Direction.up: _direction = transform.up; break;
            case Direction.down: _direction = -transform.up; break;
            case Direction.right: _direction = transform.right; break;
            case Direction.left: _direction = -transform.right; break;
        }
    }
    private void Start()
    {
        point = transform.position;
        raycastHits = Physics.CapsuleCastAll(point, point, radius, _direction, maxDistance);
        //System.Array.Clear(raycastHitsObjects, 0, 0);
        System.Array.Resize(ref raycastHitsObjects, raycastHits.Length);
        for (int i = 0; i < raycastHits.Length; i++)
        {
            raycastHitsObjects[i] = raycastHits[i].transform.gameObject;
        }
        GameObject Enemy = raycastHitsObjects.OrderBy((GameObject e) => //SearchTag変数内のtagを持ったobjectを配列化、近い順に整列
        {
            float distance = float.MaxValue;
            if (transform.position.z < e.transform.position.z && e.CompareTag(SearchTag)) distance = Vector3.Distance(transform.position, e.transform.position);
            return distance;
        }).FirstOrDefault();
        if (Enemy != null && Enemy.CompareTag(SearchTag))
        {
            target = Enemy;
            Debug.Log(target);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null && target.activeSelf == false) target = null;
        if (target != null)// 最寄りのオブジェクトが見つかった場合の処理
        {
            target_lost = true;
            //対象の位置の方向を向く
            transform.LookAt(target.transform);
            //自分自身の位置から相対的に移動する
            transform.Translate(Vector3.forward * 50 * Time.deltaTime);
        }
        if (target && target.activeSelf == false)
        {
            target = null;
        }
        if (target_lost && target == null) transform.Translate(Vector3.forward * 50 * Time.deltaTime);
    }
}
