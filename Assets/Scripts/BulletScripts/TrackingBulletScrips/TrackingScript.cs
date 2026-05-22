using UnityEngine;

public class TrackingScript : MonoBehaviour
{
    [SerializeField] string SearchTag="Enemy";
    public float SearchDistance = 2f;//1以下にすると追尾がほぼ無効になる
    float SearchDirectivity = 1f;//この値を上げると指向性が上がるが不具合の元となる、触るな危険

    void Update()
    {
        GameObject nearestObj = GetNearestObjectWithTag(SearchTag);

        if (nearestObj != null)// 最寄りのオブジェクトが見つかった場合の処理
        {
            //対象の位置の方向を向く
            transform.LookAt(nearestObj.transform);
            //自分自身の位置から相対的に移動する
            transform.Translate(Vector3.forward * 100*Time.deltaTime);
        }
        
    }

    GameObject GetNearestObjectWithTag(string tagName)
    {
        // 指定したタグを持つオブジェクトをすべて配列で取得
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagName);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject obj in taggedObjects)
        {
            if (gameObject.transform.position.z < obj.transform.position.z+SearchDirectivity)//自身よりも後ろの敵を追尾しない
            {
                // 距離を計算
                float distance = Vector3.Distance(currentPos, obj.transform.position);

                // 現在の最小距離より小さければ&直線距離がSearchDistanceよりも短ければ更新
                if (distance < minDistance&&distance<=SearchDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
        }
        return nearest;
    }
}