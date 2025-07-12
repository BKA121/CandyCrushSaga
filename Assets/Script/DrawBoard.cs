using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using System;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class DrawBoard : MonoBehaviour
{
    [SerializeField] public GameObject tilePerfab;
    [SerializeField] public GameObject wallPerfab, floorPerfab;
    [SerializeField] public GameObject candyRedPrefab, candyBluePrefab, candyGreenPrefab;
    public float distanceTile = 1.02f;
    FirebaseFirestore db;

    private void Start()
    {
        // kết nối tới firestore 
        db = FirebaseFirestore.DefaultInstance;
        LoadLevel("level_1");
    }

    private void LoadLevel(string id_level)
    {
        // đường link tới dữ liệu, đây chưa có dữ liệu, mới chỉ là địa chỉ tới dữ liệu 
        var docRef = db.Collection("levels").Document("level_1");

        /* yêu cầu server tải dữ liệu lên firestore, firestore không chứa dữ liệu ngay lập tức
           nên phải gọi lệnh GetSnapshotAsync, khi có dữ liệu phản hồi về mới thực hiện công việc tiếp 
        */
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(ReadData);
    }

    private void ReadData(Task<DocumentSnapshot> task)
    {
        if (task.IsCompleted)
        {
            // tạo ra bản chụp dữ liệu snapshot từ kết quả của task 
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> data = snapshot.ToDictionary();
                if (data.ContainsKey("board"))
                {
                    Dictionary<string, object> board = data["board"] as Dictionary<string, object>;
                    int row = int.Parse(board["row"].ToString());
                    int column = int.Parse(board["column"].ToString());
                    drawBoard(row, column);

                    for(int i=1; i <= row * column; i++)
                    {
                        string cellKey = "cell" + i;
                        if (board.ContainsKey(cellKey))
                        {
                            var cell = board[cellKey] as Dictionary<string, object>;
                            int x = int.Parse(cell["x"].ToString());
                            int y = int.Parse(cell["y"].ToString());
                            string content = cell["content"].ToString();
                            drawCandy(x, y, content);
                        }
                    }
                }
            }
        }
    }
    private void drawCandy(int x, int y, string content)
    {
        Vector2 positionCandy = new Vector2(x * distanceTile, y * distanceTile);
        if (content == "R")
        {
            GameObject candyR = Instantiate(candyRedPrefab, positionCandy, Quaternion.identity);
        }
        if (content == "B")
        {
            GameObject candyB = Instantiate(candyBluePrefab, positionCandy, Quaternion.identity);
        }
        if (content == "G")
        {
            GameObject candyG = Instantiate(candyGreenPrefab, positionCandy, Quaternion.identity);
        }
    }
    private void drawBoard(int row, int column)
    {
        for(int i=0; i<column; i++)
        {
            for(int j=0; j<row; j++)
            {
                Vector2 position = new Vector2(i * distanceTile, j * distanceTile);
                GameObject tile = Instantiate(tilePerfab, position, Quaternion.identity);
                tile.transform.SetParent(this.transform);

                if (j == 0)
                {
                    Vector2 positionFloor = new Vector2(i * distanceTile, -distanceTile/2);
                    GameObject floor = Instantiate(floorPerfab, positionFloor, Quaternion.identity);
                    floor.transform.SetParent(this.transform);
                }

                if (i == 0)
                {
                    Vector2 positionWallLeft = new Vector2(-distanceTile / 2, j * distanceTile);
                    GameObject wallLeft = Instantiate(wallPerfab, positionWallLeft, Quaternion.identity);
                    wallLeft.transform.SetParent(this.transform);
                }

                if (i == column - 1)
                {
                    Vector2 positionWallRight = new Vector2(i * distanceTile + distanceTile/2, j * distanceTile);
                    GameObject wallRight = Instantiate(wallPerfab, positionWallRight, Quaternion.identity);
                    wallRight.transform.SetParent(this.transform);
                }
            }
        }

    }
}
