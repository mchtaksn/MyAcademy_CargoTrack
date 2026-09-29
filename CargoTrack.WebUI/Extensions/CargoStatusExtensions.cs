using System.Globalization;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.WebUI.Extensions
{
    public static class CargoStatusExtensions
    {
        public static string ToDisplayText(this CargoStatus status) => status switch
        {
            CargoStatus.Received => "Şubede Kabul Edildi",
            CargoStatus.InTransferCenter => "Transfer Merkezinde",
            CargoStatus.DispatchedFromTransferCenter => "Transfer Merkezinden Çıktı",
            CargoStatus.ArrivedAtDeliveryBranch => "Varış Şubesinde",
            CargoStatus.OutForDelivery => "Dağıtıma Çıktı",
            CargoStatus.Delivered => "Teslim Edildi",
            CargoStatus.FailedDelivery => "Teslim Edilemedi",
            CargoStatus.InReturnProcess => "İade Sürecinde",
            CargoStatus.ReturnedToSender => "Göndericiye İade Edildi",
            _ => status.ToString()
        };

        public static string ToDisplayText(this CargoType type) => type switch
        {
            CargoType.Standard => "Standart Gönderi",
            CargoType.Urgent => "Acil Gönderi",
            CargoType.Fragile => "Kırılabilir Gönderi",
            CargoType.Heavy => "Ağır Gönderi",
            _ => type.ToString()
        };

        public static string ToIcon(this CargoStatus status) => status switch
        {
            CargoStatus.Received => "inventory_2",
            CargoStatus.InTransferCenter => "swap_horiz",
            CargoStatus.DispatchedFromTransferCenter => "moving",
            CargoStatus.ArrivedAtDeliveryBranch => "storefront",
            CargoStatus.OutForDelivery => "local_shipping",
            CargoStatus.Delivered => "task_alt",
            CargoStatus.FailedDelivery => "error",
            CargoStatus.InReturnProcess => "undo",
            CargoStatus.ReturnedToSender => "assignment_return",
            _ => "package_2"
        };

        public static bool IsFinal(this CargoStatus status) =>
            status is CargoStatus.Delivered or CargoStatus.ReturnedToSender;

        public static bool IsProblem(this CargoStatus status) =>
            status is CargoStatus.FailedDelivery or CargoStatus.InReturnProcess or CargoStatus.ReturnedToSender;

        public static string ToFilterGroup(this CargoStatus status) =>
            status == CargoStatus.Delivered ? "delivered" : status.IsProblem() ? "problem" : "transit";

        public static int ToProgressPercent(this CargoStatus status) => status switch
        {
            CargoStatus.Received => 10,
            CargoStatus.InTransferCenter => 30,
            CargoStatus.DispatchedFromTransferCenter => 50,
            CargoStatus.ArrivedAtDeliveryBranch => 65,
            CargoStatus.OutForDelivery => 80,
            CargoStatus.Delivered => 100,
            CargoStatus.FailedDelivery => 80,
            CargoStatus.InReturnProcess => 80,
            CargoStatus.ReturnedToSender => 100,
            _ => 0
        };

        public static string ToBadgeColorClass(this CargoStatus status) =>
            status == CargoStatus.Delivered ? "text-teal bg-teal/10 border-teal/30"
            : status.IsProblem() ? "text-rust bg-rust/10 border-rust/30"
            : "text-amber bg-amber/10 border-amber/30";

        public static string ToSolidClass(this CargoStatus status) =>
            status == CargoStatus.Delivered ? "bg-teal text-paper"
            : status.IsProblem() ? "bg-rust text-paper"
            : "bg-amber text-ink";

        public static string ToAccentBarClass(this CargoStatus status) =>
            status == CargoStatus.Delivered ? "bg-teal" : status.IsProblem() ? "bg-rust" : "bg-amber";

        public static string ToRouteFillClass(this CargoStatus status) =>
            status == CargoStatus.Delivered ? "bg-teal"
            : status.IsProblem() ? "bg-rust"
            : "bg-gradient-to-r from-teal to-amber";

        public static string ToRingColor(this CargoStatus status) =>
            status.IsProblem() ? "#C1523B" : "#E8A93B";

        public static string ToHint(this CargoStatus status) => status switch
        {
            CargoStatus.Received => "Gönderi şubede kabul edildi, yola çıkmak üzere.",
            CargoStatus.InTransferCenter => "Gönderi transfer merkezinde ayrıştırılıyor.",
            CargoStatus.DispatchedFromTransferCenter => "Gönderi varış şubesine doğru yolda.",
            CargoStatus.ArrivedAtDeliveryBranch => "Gönderi varış şubesinde, dağıtıma hazırlanıyor.",
            CargoStatus.OutForDelivery => "Kurye dağıtımda, gün içinde teslim edilecek.",
            CargoStatus.Delivered => "Gönderi alıcısına teslim edildi.",
            CargoStatus.FailedDelivery => "Teslimat denemesi başarısız oldu, yeniden dağıtım planlanacak.",
            CargoStatus.InReturnProcess => "Gönderi göndericiye iade ediliyor.",
            CargoStatus.ReturnedToSender => "Gönderi göndericiye iade edildi.",
            _ => ""
        };

        public static string ToAdminBadgeClass(this CargoStatus status) => status switch
        {
            CargoStatus.Received => "bg-slate-100 text-slate-600",
            CargoStatus.InTransferCenter => "bg-indigo-50 text-indigo-700",
            CargoStatus.DispatchedFromTransferCenter => "bg-blue-50 text-blue-700",
            CargoStatus.ArrivedAtDeliveryBranch => "bg-teal-50 text-teal-700",
            CargoStatus.OutForDelivery => "bg-secondary-container/15 text-secondary",
            CargoStatus.Delivered => "bg-success-container text-success",
            CargoStatus.FailedDelivery => "bg-rose-50 text-rose-700",
            CargoStatus.InReturnProcess => "bg-error-container text-on-error-container",
            CargoStatus.ReturnedToSender => "bg-slate-100 text-slate-500",
            _ => "bg-surface-container text-on-surface-variant"
        };

        public static string ToDotColor(this CargoStatus status) => status switch
        {
            CargoStatus.InTransferCenter => "bg-indigo-500",
            CargoStatus.DispatchedFromTransferCenter => "bg-blue-500",
            CargoStatus.OutForDelivery => "bg-secondary-container",
            CargoStatus.InReturnProcess => "bg-error",
            _ => ""
        };

        public static bool HasActivePulse(this CargoStatus status) =>
            status is CargoStatus.InTransferCenter or CargoStatus.DispatchedFromTransferCenter
                   or CargoStatus.OutForDelivery or CargoStatus.InReturnProcess;
    }

    public static class DisplayFormatExtensions
    {
        private static readonly CultureInfo Tr = new("tr-TR");

        // Veritabanında UTC tutuluyor, Türkiye UTC+3 (yaz saati yok)
        private static DateTime ToTurkeyTime(this DateTime utc) => utc.AddHours(3);

        public static string ToShortTr(this DateTime utc) => utc.ToTurkeyTime().ToString("d MMM", Tr);

        public static string ToLongTr(this DateTime utc) => utc.ToTurkeyTime().ToString("d MMMM yyyy, dddd", Tr);

        public static string ToDateTimeTr(this DateTime utc) => utc.ToTurkeyTime().ToString("d MMM · HH:mm", Tr);

        public static string ToRelativeDay(this DateTime utc)
        {
            var day = utc.ToTurkeyTime().Date;
            var today = DateTime.UtcNow.ToTurkeyTime().Date;

            if (day == today) return "Bugün";
            if (day == today.AddDays(1)) return "Yarın";
            if (day == today.AddDays(-1)) return "Dün";
            return day.ToString("d MMM", Tr);
        }

        public static string ToTrNumber(this double value) => value.ToString("0.#", Tr);

        public static string ToInitials(this string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "?";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? parts[0][..1].ToUpper(Tr)
                : (parts[0][..1] + parts[^1][..1]).ToUpper(Tr);
        }

        public static string ToTurkeyPrintStamp(this DateTime utc) => utc.ToTurkeyTime().ToString("d MMMM yyyy, HH:mm", Tr);
    }
}