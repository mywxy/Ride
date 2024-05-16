using System;
using System.Windows.Forms;
using RacingGame;

public static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        GameModel model = new GameModel();
        GameView view = new GameView();
        GameController controller = new GameController(model, view);

        view.ShowStartMenu();
        Application.Run(view);
    }
}
