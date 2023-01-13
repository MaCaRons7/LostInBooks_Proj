public class EnemyHeap
{
    private EnemySpawner.Controller[] heap = new EnemySpawner.Controller[100];
    private int num;
    private void Swap(ref EnemySpawner.Controller x,ref EnemySpawner.Controller y)
    {
        EnemySpawner.Controller temp;
        temp = x;
        x = y;
        y = temp;
    }

    public int GetNum()
    {
        return num;
    }

    public EnemySpawner.Controller GetTop()
    {
        return heap[1];
    }

    public void Insert(EnemySpawner.Controller controller)
    {
        heap[++num] = controller;
        HeapUp(num);
    }

    public void ChangeTop(float x)
    {
        heap[1].beginTime = x;
        heap[1].num--;
        if (heap[1].num <= 0) heap[1].beginTime = 10000;
        HeapDown(1);
    }

    private void HeapUp(int now)
    {
        if (now / 2 > 0 && heap[now / 2].beginTime > heap[now].beginTime)
        {
            Swap(ref heap[now], ref heap[now/2]);
            HeapUp(now / 2);
        }
    }

    private void HeapDown(int now)
    {
        if (now * 2 <= num && heap[now * 2].beginTime < heap[now].beginTime)
        {
            if (heap[now * 2].beginTime < heap[now * 2 + 1].beginTime || now * 2 + 1 > num)
            {
                Swap(ref heap[now * 2], ref heap[now]);
                HeapDown(now * 2);
            }
            else
            {
                Swap(ref heap[now * 2 + 1], ref heap[now]);
                HeapDown(now * 2 + 1);
            }
        }
        else if (now * 2 + 1 <= num && heap[now * 2 + 1].beginTime < heap[now].beginTime)
        {
            Swap(ref heap[now * 2 + 1], ref heap[now]);
            HeapDown(now * 2 + 1);
        }
    }

    public void Clear()
    {
        num = 0;
    }
}
