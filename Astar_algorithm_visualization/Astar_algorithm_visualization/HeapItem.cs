/*
    Heap.cs와 HeapItem.cs 부분인 Heap구조 클래스는
    http://periar.tistory.com/entry/C-%EC%9A%B0%EC%84%A0%EC%88%9C%EC%9C%84-%ED%81%90
    에서 가져와 시각화 용도에 맞게(프로젝트 목표에 맞게) 일부 수정했습니다.
    해당 출처가 원 저작자의 출처인지는 알 수 없습니다.
*/
class HeapItem
{
    public int Ranking { get; set; }
    public object Value { get; set; }

    public HeapItem(int rank, object value)
    {
        this.Ranking = rank;
        this.Value = value;
    }
}