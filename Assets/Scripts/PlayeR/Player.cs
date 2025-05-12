using System.Collections;
using TMPro;
using UnityEngine;

public class Player : Entity
{
    private Camera _main;
    private const float MOVE_CD = 0.5f;
    private const float MOVE_DISPLACEMENT = 0.8f;
    private void Awake()
    {
        StartCoroutine(Move());
        _main = Camera.main;
    }
    private IEnumerator Move()
    {
        while(this)
        {
            yield return new WaitForSeconds(MOVE_CD);
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                int nextX = GridPosition.x + (int)Input.GetAxisRaw("Horizontal");
                int nextY = GridPosition.y + (int)Input.GetAxisRaw("Vertical");
                TryDisplace(new Vector2Int(nextX, nextY));
            }
            if (Input.GetMouseButton(0))
            {
                Vector2 dir = MaxContrast((_main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized);
                TryDisplace(new Vector2Int(GridPosition.x + (int)dir.x, GridPosition.y + (int)dir.y));
            }
        }
    }
    public void TryDisplace(Vector2Int toWhere)
    {
        int nextX = toWhere.x;
        int nextY = toWhere.y;
        if (!CheckPlace(nextX, nextY, out int error))
        {
            if (error != -1)
                SquareCreator.map[nextX, nextY].currentEntity.Interact();
            return;
        }
        transform.position = new Vector3(nextX * MOVE_DISPLACEMENT, nextY * MOVE_DISPLACEMENT, 0f);
        SquareCreator.map[GridPosition.x, GridPosition.y].currentEntity = null;
        GridPosition.x = nextX;
        GridPosition.y = nextY;
        SquareCreator.map[GridPosition.x, GridPosition.y].currentEntity = this;
        StartCoroutine(LerpCamera(transform.position));
    }
    private IEnumerator LerpCamera(Vector2 where)
    {
        float currentStep = 0f;
        Vector3 dir = where - (Vector2)_main.transform.position;
        while(currentStep < MOVE_CD)
        {
            yield return new WaitForSeconds(MOVE_CD / 5);
            currentStep += MOVE_CD / 5;
            _main.transform.position += dir / 5;
        }
        yield break;
    }
    private bool CheckPlace(int x, int y, out int error)
    {
        if (x < 0 || y < 0 || y >= SquareCreator.MAP_HEIGHT || x >= SquareCreator.MAP_WIDTH)
        {
            error = -1;
            return false;
        }
        error = 0;
        return true;
    }
    private bool CheckPlace(int x, int y) =>
        CheckPlace(x, y, out int error);
    public override void Interact()
    {
        throw new System.NotImplementedException();
    }
    private Vector2 MaxContrast(Vector2 vec)
    {
        if (vec.x == vec.y) return Vector2.zero;
        if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
            return new Vector2(vec.x > 0 ? 1 : -1, 0);
        return new Vector2(0, vec.y > 0 ? 1 : -1);
    }
}
