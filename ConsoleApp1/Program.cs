﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CodeSmellsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderService = new OrderService(new PriceCalculator(), new DiscountService(), new OrderRepository());
            var processor = new OrderProcessor(orderService, new Logger());

            processor.ProcessOrder("John Doe", 5, "standard");
            processor.ProcessOrder("Jane Doe", 15, "premium");
            processor.ProcessOrder("Alice", 2, "standard");
        }
    }

    class OrderProcessor
    {
        private readonly OrderService _orderService;
        private readonly Logger _logger;

        public OrderProcessor(OrderService orderService, Logger logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public void ProcessOrder(string customer, int quantity, string customerType)
        {
            double finalPrice = _orderService.HandleOrder(customer, quantity, customerType);
            Console.WriteLine($"Customer: {customer}, Quantity: {quantity}, Total Price: {finalPrice}");
            _logger.Log($"Processed order for {customer}, quantity: {quantity}, price: {finalPrice}, type: {customerType}");
        }
    }

    class OrderService
    {
        private readonly PriceCalculator _calculator;
        private readonly DiscountService _discountService;
        private readonly OrderRepository _orderRepository;

        public OrderService(PriceCalculator calculator, DiscountService discountService, OrderRepository orderRepository)
        {
            _calculator = calculator;
            _discountService = discountService;
            _orderRepository = orderRepository;
        }

        public double HandleOrder(string customer, int quantity, string customerType)
        {
            double price = _calculator.CalculatePrice(quantity);
            price = _discountService.ApplyDiscount(price, customerType);
            _orderRepository.SaveOrder(customer, quantity, price);
            return price;
        }
    }

    class OrderRepository
    {
        private const string FilePath = "orders.txt";

        public void SaveOrder(string customer, int quantity, double price)
        {
            File.AppendAllText(FilePath, $"{customer}, {quantity}, {price}\n");
        }
    }

    class PriceCalculator
    {
        public double CalculatePrice(int quantity)
        {
            double price = quantity * 10;
            if (quantity > 10)
            {
                price *= 0.9; 
            }
            return price;
        }
    }

    class DiscountService
    {
        public double ApplyDiscount(double price, string customerType)
        {
            if (customerType == "premium")
            {
                return price * 0.85; 
            }
            return price;
        }
    }

    class Logger
    {
        private List<string> logs = new List<string>();

        public void Log(string message)
        {
            logs.Add(message);
            Console.WriteLine($"LOG: {message}");
        }
    }
}
