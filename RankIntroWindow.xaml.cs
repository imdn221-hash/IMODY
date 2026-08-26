using System.Windows;

namespace IMODY
{
    public partial class RankIntroWindow : Window
    {
        public RankIntroWindow()
        {
            InitializeComponent();
        }

        private void StartAdventure_Click(object sender, RoutedEventArgs e)
        {
            UserSession.HasSeenRankIntro = true;
            UserSession.SaveSession();
            Close();
        }
    }
}
