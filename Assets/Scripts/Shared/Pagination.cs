using UnityEngine;
using UnityEngine.UI;


public class Pagination : MonoBehaviour
{
    protected const int CONTAINER_SIZE = 10;

    [SerializeField] protected GridLayoutGroup _cardContainer;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;

    protected int _currentPage = 0;
    protected int _maxPage = 1;

    public void NextPage()
    {
        _currentPage += 1;
        Render();
    }

    public void PreviousPage()
    {
        _currentPage -= 1;
        Render();
    }

    protected virtual void Render()
    {
        _previousButton.gameObject.SetActive(_currentPage > 0);
        _nextButton.gameObject.SetActive(_currentPage < _maxPage);

        // clear container
        for (int i = 0; i < _cardContainer.transform.childCount; i++)
        {
            Destroy(_cardContainer.transform.GetChild(i).gameObject);
        }

        RenderCard();
    }

    protected virtual void RenderCard() { }
}