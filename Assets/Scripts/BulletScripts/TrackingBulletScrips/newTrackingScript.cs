using System.Linq;
using UnityEngine;

public class newTrackingScript : MonoBehaviour
{
    [SerializeField] string SearchTag="Enemy";
    GameObject Target=null;
    public float SearchDistance = 2f;

    void Awake()
    {
        GameObject Enemy = GameObject.FindGameObjectsWithTag(SearchTag).OrderBy((GameObject e) => //SearchTag変数内のtagを持ったobjectを配列化、近い順に整列
        { 
            float distance = float.MaxValue; 
            Vector2 num = e.transform.position - transform.position; 
            if (transform.position.z < e.transform.position.z && //自身の前後の円柱範囲内に居るかつ自身よりも前に居るobjectの自身からの距離を抽出
            Mathf.Abs(num.sqrMagnitude) < SearchDistance * SearchDistance) distance = Vector3.Distance(transform.position, e.transform.position); 
            return distance; 
        }).FirstOrDefault();//最終的に円柱範囲内から最も自身に近い対象をEnemyに代入する
        if(Enemy != null)
        {
            Vector2 num = (Enemy.transform.position - transform.position);
            //Debug.Log(num.x+" "+ num.y); Enemyに格納されている変数は円柱範囲内の自身から最も近い対象の情報なので、ここでもう一度円柱範囲の処理をしておかないと追尾範囲が無限になる
            if (Mathf.Abs(num.sqrMagnitude) < SearchDistance * SearchDistance) Target = Enemy;
        }
    }
    void Update()
    {
        if (Target != null)// 最寄りのオブジェクトが見つかった場合の処理
        {
            //対象の位置の方向を向く
            transform.LookAt(Target.transform);
            //自分自身の位置から相対的に移動する
            transform.Translate(Vector3.forward * 50*Time.deltaTime);
        }
    }
}