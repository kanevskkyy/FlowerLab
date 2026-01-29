using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.Domain.Entities;

namespace OrderService.BLL.Profiles
{
    public class OrderStatusProfile : Profile
    {
        public OrderStatusProfile()
        {
            CreateMap<OrderStatus, OrderStatusReadDto>()
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => GetTranslations(src.Name)));

            CreateMap<OrderStatusCreateDto, OrderStatus>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()));

            CreateMap<OrderStatusUpdateDto, OrderStatus>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }

        private Dictionary<string, string> GetTranslations(string statusName)
        {
            if (string.IsNullOrEmpty(statusName)) return new Dictionary<string, string>();
            var normalized = System.Text.RegularExpressions.Regex.Replace(statusName, @"\s+", "").ToLower();
            
            string canonicalKey;
            switch (normalized)
            {
                case "new":
                case "pending":
                case "новий":
                case "очікує":
                case "очікуєвиконання":
                case "awaitingexecution":
                    canonicalKey = "pending";
                    break;
                case "processing":
                case "вобробці":
                    canonicalKey = "processing";
                    break;
                case "shipped":
                case "delivering":
                case "indelivery":
                case "доставляється":
                    canonicalKey = "delivering";
                    break;
                case "delivered":
                case "completed":
                case "виконано":
                case "доставлено":
                    canonicalKey = "completed";
                    break;
                case "cancelled":
                case "скасовано":
                    canonicalKey = "cancelled";
                    break;
                case "awaitingpayment":
                case "очікуєоплати":
                    canonicalKey = "awaitingpayment";
                    break;
                case "paymentfailed":
                case "оплатаневдалася":
                    canonicalKey = "paymentfailed";
                    break;
                case "wait_accept":
                case "waitaccept":
                case "очікуєпідтвердження":
                    canonicalKey = "wait_accept";
                    break;
                case "confirmed":
                case "підтверджено":
                    canonicalKey = "confirmed";
                    break;
                default:
                    canonicalKey = normalized;
                    break;
            }

            var dict = new Dictionary<string, string>
            {
                { "en", statusName },
                { "ua", statusName }
            };

            switch (canonicalKey)
            {
                case "new":
                    dict["en"] = "New";
                    dict["ua"] = "Нове";
                    break;
                case "pending":
                    dict["en"] = "Pending";
                    dict["ua"] = "Очікує виконання";
                    break;
                case "processing":
                    dict["en"] = "Processing";
                    dict["ua"] = "В обробці";
                    break;
                case "delivering":
                    dict["en"] = "Delivering";
                    dict["ua"] = "Доставляється";
                    break;
                case "completed":
                    dict["en"] = "Completed";
                    dict["ua"] = "Виконано";
                    break;
                case "cancelled":
                    dict["en"] = "Cancelled";
                    dict["ua"] = "Скасовано";
                    break;
                case "awaitingpayment":
                    dict["en"] = "Awaiting Payment";
                    dict["ua"] = "Очікує оплати";
                    break;
                case "paymentfailed":
                    dict["en"] = "Payment Failed";
                    dict["ua"] = "Оплата не вдалася";
                    break;
                case "wait_accept":
                    dict["en"] = "Wait Accept";
                    dict["ua"] = "Очікує підтвердження";
                    break;
                case "confirmed":
                    dict["en"] = "Confirmed";
                    dict["ua"] = "Підтверджено";
                    break;
            }
            return dict;
        }
    }
}
