using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SudokuSolver {
    string path = "Assets/Sudoku files/sudoku.txt";
    string sudokuString;

    List<SudokuSlot> openSlots = new List<SudokuSlot>();
    int[, ] sudokuGrid = new int[9, 9];
    public SudokuUI mySudokuUI;

    int tries;
    Stopwatch simulatedTime = new Stopwatch();
    Stopwatch realTime = new Stopwatch();

    /// <summary>
    /// Clears the grid
    /// </summary>
    public void ClearGrid() {
        for (int i = 0; i < 9; i++) {
            for (int e = 0; e < 9; e++) {
                sudokuGrid[e, i] = 0;
                mySudokuUI.SetSudokuUI((i * 9) + e, 0);
            }
        }

        openSlots = new List<SudokuSlot>();
    }

    //Converts the text file into a simple string of numbers and converts it to a grid
    public void GetSudokuFiles() {
        ClearGrid();

        StreamReader reader = new StreamReader(path);
        sudokuString = reader.ReadToEnd();
        reader.Close();

        sudokuString = sudokuString.Replace(" ", string.Empty);
        sudokuString = sudokuString.Replace("\n", string.Empty);
        sudokuString = sudokuString.Replace("\r", string.Empty);
        sudokuString = sudokuString.Replace("\t", string.Empty);

        for (int i = 0; i < 9; i++) {
            for (int e = 0; e < 9; e++) {
                sudokuGrid[e, i] = int.Parse(sudokuString.ToCharArray()[(i * 9) + e].ToString());
                mySudokuUI.SetSudokuUI((i * 9) + e, sudokuGrid[e, i]);
            }
        }
    }

    //Adds all possibilities to a list
    void StartSolve() {
        simulatedTime.Restart();
        realTime.Restart();
        tries = 0;
        GetSudokuFiles();

        for (int y = 0; y < 9; y++) {
            for (int x = 0; x < 9; x++) {
                if (sudokuGrid[x, y] == 0) {
                    SudokuSlot newSlot = new SudokuSlot(x, y);

                    for (int n = 1; n < 10; n++) {
                        if (checkPosition(n, x, y)) {
                            newSlot.possibilities.Add(n);
                        }
                    }

                    openSlots.Add(newSlot);
                }
            }
        }
        realTime.Stop();
    }

    //Checks if the possibilitie fits in the in the slot
    bool CheckPossibilities(int i) {
        tries++;

        for (int p = openSlots[i].currentAttempt; p < openSlots[i].possibilities.Count; p++) {
            if (checkPosition(openSlots[i].possibilities[p], openSlots[i].slotX, openSlots[i].slotY)) {
                sudokuGrid[openSlots[i].slotX, openSlots[i].slotY] = openSlots[i].possibilities[p];
                mySudokuUI.SetSudokuUI((openSlots[i].slotY * 9) + openSlots[i].slotX, openSlots[i].possibilities[p]);
                openSlots[i].currentAttempt = p;
                return true;
            }
        }

        openSlots[i].currentAttempt = 0;
        return false;
    }

    //Checks if the possibilitie is possible on the given position
    bool checkPosition(int toCheck, int horPosition, int verPosition) {

        for (int i = 0; i < 9; i++) {
            if (sudokuGrid[i, verPosition] == toCheck || sudokuGrid[horPosition, i] == toCheck) {
                return false;
            }
        }

        horPosition = horPosition - horPosition % 3;
        verPosition = verPosition - verPosition % 3;

        for (int y = 0; y < 3; y++) {
            for (int x = 0; x < 3; x++) {
                if (sudokuGrid[x + horPosition, y + verPosition] == toCheck) {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Solves the Sudoku
    /// </summary>
    /// <param name="secondsToWait">Seconds to wait until next slot calculation</param>
    /// <returns></returns>
    public IEnumerator Solve(float secondsToWait) {
        StartSolve();

        for (int i = 0; i < openSlots.Count; i++) {
            realTime.Stop();
            yield return new WaitForSeconds(secondsToWait);
            realTime.Start();
            if (!CheckPossibilities(i)) {
                if ((i - 1) >= 0) {
                    sudokuGrid[openSlots[i - 1].slotX, openSlots[i - 1].slotY] = 0;
                    mySudokuUI.SetSudokuUI((openSlots[i - 1].slotY * 9) + openSlots[i - 1].slotX, 0);

                    openSlots[i - 1].currentAttempt++;
                    i -= 2;
                }
                else {
                    Complete(false);
                }
            }
        }
        Complete(true);
    }

    //Triggers compleation UI
    void Complete(bool state) {
        simulatedTime.Stop();
        mySudokuUI.CompleteUI(tries, state, realTime.Elapsed.Milliseconds.ToString(), (simulatedTime.Elapsed.Milliseconds / 100).ToString());
    }
}

//Class with information about slots with possible solutions
class SudokuSlot {
    public int slotX, slotY;
    public List<int> possibilities = new List<int>();
    public int currentAttempt = 0;

    public SudokuSlot(int x, int y) {
        slotX = x;
        slotY = y;
    }
}