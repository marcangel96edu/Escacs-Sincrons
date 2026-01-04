using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;

public class Simulator : MonoBehaviour 
{
    private string path;

    public void setFileName(string fileName) {
        string directory = Path.Combine(Application.dataPath, "Results");

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        path = Path.Combine(directory, fileName);

        if (!File.Exists(path))
            File.WriteAllText(path, "gameId,enemyAlg,playerAlg,result,turns\n");
    }

    public void LogGame(int gameId, int enemyAlg, int playerAlg, string result, int turns) {
        string line = $"{gameId},{enemyAlg},{playerAlg},{result},{turns}\n";
        File.AppendAllText(path, line);
    }
}