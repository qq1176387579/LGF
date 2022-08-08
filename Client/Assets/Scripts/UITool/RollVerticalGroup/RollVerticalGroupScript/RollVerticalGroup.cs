using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGF.DataStruct;

public interface RollVerticalGroupItem<T>
{
    void RefreshData(int itemidx,T info);
}

public class RollVerticalGroup<T> : MonoBehaviour
{
    [SerializeField] private Transform content = null;
    [Header("item间隔")]
    [SerializeField] private float itemSpacing = 0;
    [Header("item高度")]
    [SerializeField] private float itemHeight;

    [Header("item Prefab")]
    public Transform itemPrefab;

    List<T> m_infoList = new List<T>();
    int itemCount { get => m_infoList == null ? 0 : m_infoList.Count; }
    float m_maxheight;
    RectTransform m_rect, m_contentRect;

    //float m_lastPosY; //上次位置

    //RectTransform parentRect;

    float GetPosY { get => m_contentRect.anchoredPosition.y; }
    float GetPosX { get => m_contentRect.anchoredPosition.x; }

    float sizeY { get => m_rect.sizeDelta.y; }
    float sizeX { get => m_rect.sizeDelta.x; }

    bool m_isInit = false;
    public void Init()
    {
        if (m_isInit) return;
        m_isInit = true;
        m_rect = transform as RectTransform;
        m_contentRect = content as RectTransform;
        gameObject.GetComponent<ScrollRect>().onValueChanged.AddListener(OnScrollRectValueChanged);
        if (0 == itemHeight)
        {
            RectTransform itemRect = (itemPrefab as RectTransform);
            itemHeight = itemRect.sizeDelta.y;
        }
    }

    private void Start()
    {
        Init();
    }


    public void InitInfo(List<T> infoList, bool isInitLocation = true)    //初始化数据
    {
        if (isInitLocation)
            InitLocation();

        m_infoList.Clear();
        for (int i = 0; i < infoList.Count; i++)
            m_infoList.Add(infoList[i]);

        ClearItem();
        InitMaxheight();
        OnScrollRect();
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void RefreshAllItemInfo()
    {
        linkedList.RefreshData<List<T>>(m_infoList);
    }

    void InitLocation()
    {
        m_contentRect.anchoredPosition = new Vector2(GetPosX, 0);
    }

    void InitMaxheight()  //计算高度
    {
        m_maxheight = 0;
        m_maxheight = itemCount * (itemSpacing + itemHeight) - itemSpacing;
        m_contentRect.sizeDelta = new Vector2(m_rect.sizeDelta.x, m_maxheight);   //高度
    }

    #region item池

    Deque<ItemInfoClass> linkedList = new Deque<ItemInfoClass>();
    Stack<ItemInfoClass> itemPool = new Stack<ItemInfoClass>();

    class ItemInfoClass : IOnRefreshData<List<T>>
    {
        public int id;
        public RectTransform rect;
        public RollVerticalGroupItem<T> info;

        public void RefreshData(List<T> t1)
        {
            info?.RefreshData(id, t1[id]);
        }

        public override string ToString()
        {
            return id.ToString();
        }
    }

    void RecycleItem(ItemInfoClass item)
    {
        item.rect.gameObject.SetActive(false);
        item.rect.transform.SetParent(gameObject.transform);
        item.id = -1;
        itemPool.Push(item);
    }

    ItemInfoClass GetItem()
    {
        RectTransform tmpRect = null;
        ItemInfoClass tmp = null;
        if (itemPool.Count > 0)
        {
            tmp = itemPool.Pop();
            tmpRect = tmp.rect;
        }
        else
        {
            tmpRect = GameObject.Instantiate(itemPrefab).transform as RectTransform;
            tmpRect.sizeDelta = new Vector2((itemPrefab as RectTransform).sizeDelta.x, itemHeight);
            tmp = new ItemInfoClass();
            tmpRect.pivot = new Vector2(0, 1);
            tmp.rect = tmpRect;
            tmp.info = tmpRect.GetComponent<RollVerticalGroupItem<T>>();
        }

        tmpRect.transform.SetParent(content);
        tmpRect.gameObject.SetActive(true);
        tmpRect.localScale = Vector3.one;
        return tmp;
    }

    #endregion

    #region Update


    //更新行
    void RefreshItem(int id)
    {
        float itemPosY = id * (itemSpacing + itemHeight);
        ItemInfoClass item = GetItem();
        item.id = id;
        item.rect.anchoredPosition3D = new Vector3(0, -itemPosY, 0);
        item.rect.gameObject.name = id.ToString();
        item.info?.RefreshData(id, m_infoList[id]);


        if (linkedList.Count == 0 || id < linkedList.GetHead().id)
        {
            linkedList.PushHead(item);
        }
        else if (id > linkedList.GetEnd().id)
        {
            linkedList.PushEnd(item);
        }
        else
        {
            Debug.LogError($"id { id} GetEnd { linkedList.GetEnd().id}  Head{linkedList.GetHead().id} 出错");
        }
    }


    void ClearItem()
    {
        while (linkedList.Count > 0)
            RecycleItem(linkedList.PopHead());
    }

    void OnScrollRect()
    {
        //0~ sizeY
        //GetPosY ~sizeY + GetPosY 
        //计算需要显示的索引 idx1 - idx2需要显示
        int idx1 = (int)((GetPosY - itemHeight) / (itemHeight + itemSpacing));      // 除了第0个 开始位置到之前有多少个;  
        int idx2 = (int)((GetPosY - itemHeight + sizeY) / (itemHeight + itemSpacing));   //除了第0个 结束位置到之前有多少个;   

        if (idx1 < 0) idx1 = 0;     //边界计算
        idx2++;                     //  多显示一个
        if (idx2 >= itemCount)      //边界判定
            idx2 = itemCount - 1;

        //Debug.Log($"idx1: {(int)idx1} idx2: {(int)idx2}  ");

        if (idx1 > idx2)    //快速跳转时  或者 idx为-2时
        {
            ClearItem();
            return;
        }
        else
        {
            while (linkedList.Count > 0 && linkedList.GetHead().id < idx1)
                RecycleItem(linkedList.PopHead());

            while (linkedList.Count > 0 && linkedList.GetEnd().id > idx2)
                RecycleItem(linkedList.PopEnd());

            if (linkedList.Count == 0)
                RefreshItem(idx1);

            while (linkedList.Count > 0 && linkedList.GetHead().id > idx1)
                RefreshItem(linkedList.GetHead().id - 1);

            while (linkedList.Count > 0 && linkedList.GetEnd().id < idx2)
                RefreshItem(linkedList.GetEnd().id + 1);

        }
    }

    void OnScrollRectValueChanged(Vector2 t) 
    {
        OnScrollRect();
    }
    #endregion
}
