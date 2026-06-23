using UnityEngine;
using System.Linq;

public class Towpointscrips : MonoBehaviour
{
    [SerializeField, Header("backの方はobj1にCamera\nobj2にAimPoint\nPlayerShotPointを代入\nnearの方はobj1にback\nobj2にsentouki\nPlayerShotPointは代入しない\nNewTrackingScriptには触れぬように")] newTrackingScript newTrackingScript;
    GameObject Target = null;
    GameObject Enemy = null;
    [SerializeField] GameObject obj1, obj2, PlayerShotPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(PlayerShotPoint != null)
        Enemy = GameObject.FindGameObjectsWithTag(newTrackingScript.SearchTag).OrderBy((GameObject e) => //SearchTag変数内のtagを持ったobjectを配列化、近い順に整列
        {
            float distance = float.MaxValue;
            Vector2 num = e.transform.position - PlayerShotPoint.transform.position;
            if (PlayerShotPoint.transform.position.z < e.transform.position.z && //自身の前後の円柱範囲内に居るかつ自身よりも前に居るobjectの自身からの距離を抽出
            Mathf.Abs(num.sqrMagnitude) < newTrackingScript.SearchDistance * newTrackingScript.SearchDistance) distance = Vector3.Distance(PlayerShotPoint.transform.position, e.transform.position);
            return distance;
        }).FirstOrDefault();//最終的に円柱範囲内から最も自身に近い対象をEnemyに代入する
        if (Enemy != null)
        {
            Vector2 num = (Enemy.transform.position - PlayerShotPoint.transform.position);
            //Debug.Log(num.x+" "+ num.y); Enemyに格納されている変数は円柱範囲内の自身から最も近い対象の情報なので、ここでもう一度円柱範囲の処理をしておかないと追尾範囲が無限になる
            if (Mathf.Abs(num.sqrMagnitude) < newTrackingScript.SearchDistance * newTrackingScript.SearchDistance) Target = Enemy;
        }
        if (Target != null) { transform.position = (obj1.transform.position + Target.transform.position) * 0.5f; Target = null; }
        else transform.position = (obj1.transform.position + obj2.transform.position) * 0.5f;
    }
}
