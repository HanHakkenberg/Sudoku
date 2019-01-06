using TMPro;
using UnityEngine;

public class SudokuUI : MonoBehaviour {
    [SerializeField] GameObject sudokuSlot;
    [SerializeField] float waitForNext = 0.2f;
    TextMeshProUGUI[] slots = new TextMeshProUGUI[81];
    SudokuSolver mySolver = new SudokuSolver();

    [SerializeField] TextMeshProUGUI triesText;
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] TextMeshProUGUI realTimeText;
    [SerializeField] TextMeshProUGUI simulatedTimeText;

    void Awake() {
        for (int i = 0; i < 81; i++) {
            GameObject l = Instantiate(sudokuSlot, transform.position, transform.rotation, transform);
            slots[i] = l.GetComponentInChildren<TextMeshProUGUI>();
        }

        mySolver.mySudokuUI = this;
    }

    /// <summary>
    /// triggers the clearance of the grid
    /// </summary>
    public void Clear() {
        StopAllCoroutines();
        mySolver.ClearGrid();
    }

    /// <summary>
    /// Sets the textfield to the given amount.
    /// </summary>
    /// <param name="slotIndex">The index in the list of Sudoku slots</param>
    /// <param name="newValue">The new value for the Sudoku slot</param>
    public void SetSudokuUI(int slotIndex, int newValue) {
        slots[slotIndex].text = newValue.ToString();
    }

    //Starts the solving of the sudoku
    public void StartSolve() {
        StopAllCoroutines();
        StartCoroutine(mySolver.Solve(waitForNext));
    }

    /// <summary>
    /// Triggers the import of the sudoku from the given location
    /// </summary>
    public void ImportSudoku() {
        StopAllCoroutines();
        mySolver.GetSudokuFiles();
    }

    /// <summary>
    /// Sets the UI With the given stats
    /// </summary>
    /// <param name="tries">The amount of tries it took to fail or succeed</param>
    /// <param name="state">The new states</param>
    /// <param name="realTime">The it took in real time</param>
    /// <param name="simulatedTime">The it took in simulated time</param>
    public void CompleteUI(int tries, bool state, string realTime, string simulatedTime) {
        triesText.text = tries.ToString();
        stateText.text = state.ToString();
        realTimeText.text = realTime + "ms";
        simulatedTimeText.text = simulatedTime + "sec";
    }
}