using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPtSystem.src.utils
{
    class Filter
    {
        public bool isEnabled { get; set; }
        public Tuple<bool, DateTime> startTime { get; set; }
        public Tuple<bool, DateTime> endTime { get; set; }
        public Tuple<bool, int> minSumValue { get; set; }
        public Tuple<bool, int> maxSumValue { get; set; }
        public Tuple<bool, string> city { get; set; }
        public Tuple<bool, string> street { get; set; }
        public Tuple<bool, string> houseNumber { get; set; }
        public Tuple<bool, string> flatNumber { get; set; }

        private List<CommunalPaymentDocument> applyDateTimeFilters(List<CommunalPaymentDocument> documents)
        {
            return documents
                .Where(v => startTime.Item1 ? DateTime.Compare(startTime.Item2, v.Date) <= 0 : true)
                .Where(v => endTime.Item1 ? DateTime.Compare(endTime.Item2, v.Date) >= 0 : true)
                .ToList();
        }

        private List<CommunalPaymentDocument> applyAddressFilters(List<CommunalPaymentDocument> documents)
        {
            return documents
                .Where(v => city.Item1 ? v.Address.City.Contains(city.Item2) : true)
                .Where(v => street.Item1 ? v.Address.Street.Contains(street.Item2) : true)
                .Where(v => houseNumber.Item1 ? v.Address.HouseNumber.ToString().StartsWith(houseNumber.Item2) : true)
                .Where(v => flatNumber.Item1 ? v.Address.FlatNumber.ToString().StartsWith(flatNumber.Item2) : true)
                .ToList();
        }
        private List<CommunalPaymentDocument> applySumValueFilters(List<CommunalPaymentDocument> documents)
        {
            return documents
                .Where(v => minSumValue.Item1 ? minSumValue.Item2 <= v.TotalSum : true)
                .Where(v => maxSumValue.Item1 ? maxSumValue.Item2 >= v.TotalSum : true)
                .ToList();
        }

        public List<CommunalPaymentDocument> applyFilters(List<CommunalPaymentDocument> documents)
        {
            if (!isEnabled)
            {
                return documents;
            }
            return applyAddressFilters(applySumValueFilters(applyDateTimeFilters(documents)));
        }
    }
}
