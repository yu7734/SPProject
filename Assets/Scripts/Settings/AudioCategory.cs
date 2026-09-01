using UnityEngine;

/// <summary>
/// その AudioSource が BGM なのか SE なのかを明示するためのタグ用コンポーネント。
///
/// AudioVolumeApplier は通常「loop が ON なら BGM / OFF なら SE」と自動判定するが、
/// ループしない BGM や、ループする環境音などは判定を間違える。
/// そういう AudioSource にこのスクリプトを付けて category を指定すると、
/// 自動判定より優先して扱われる。
///
/// 使い方：BGM を鳴らしている AudioSource のオブジェクトに Add Component して
/// 　　　　category を BGM に設定するだけ。
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioCategory : MonoBehaviour
{
    public enum Category
    {
        /// <summary>BGM（設定画面の BGM スライダーで音量が変わる）</summary>
        BGM,
        /// <summary>効果音（設定画面の SE スライダーで音量が変わる）</summary>
        SE,
    }

    [SerializeField, Tooltip("この AudioSource の種別。設定画面のどのスライダーで音量を変えるかが決まる")]
    private Category category = Category.SE;

    public Category CurrentCategory => category;
}
