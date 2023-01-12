using System.Windows;

namespace AdaptiveFEM.Services
{
    public class MessageService
    {
        public MessageService() { }

        public void SendErrorMessage(string message)
        {
            MessageBox.Show(message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public void SendSuccessMessage(string message)
        {
            MessageBox.Show(message,
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void SendInformationMessage(string message)
        {
            MessageBox.Show(message,
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void SendWarningMessage(string message)
        {
            MessageBox.Show(message,
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public bool AskBoolean(string question)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(question,
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);

            return messageBoxResult == MessageBoxResult.Yes;
        }
    }
}
