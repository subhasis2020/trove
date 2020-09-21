
namespace Foundry.Domain
{
    public enum GeneralSettingEnum
    {
        SMTP = 1,
        MailGun = 2,
        Storage = 3,
        Logging = 4
    }

    public enum AccountTypeEnum
    {
        MealPasses = 1,
        FlexSpending = 2,
        DiscretionaryPoints = 3
    }


    public enum WeekDaysEnum
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public enum Dwelltime
    {
        minutes5 = 1,
        minutes10 = 2,
        minutes15 = 3
    }

    public enum NotificationSettingsEnum
    {
        Transaction = 1,
        Awards = 2,
        Notifications = 3,
        Offers = 4,
        Personal = 5
    }
}
