
namespace Reminders.src
{
    interface ITextProvider
    {
        public void SetOutputWriter(IOutputWriter outputWriter);

        public void CheckForText();
    }
}
