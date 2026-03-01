using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "LevelNote", menuName = "Text/LevelNote", order = 1)]
public class LevelNote : SerializedScriptableObject
{
    [Title("쪽지 설정")]
    [LabelText("쪽지 제목")]
    public string noteTitle = "알 수 없는 쪽지";

    [LabelText("쪽지 내용")]
    [TextArea(7, 15)]
    public string noteText;
}
