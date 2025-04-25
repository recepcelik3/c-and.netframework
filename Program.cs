using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Set up dependency injection
        var inputService = new ConsoleInputService();
        var validationService = new PackageValidationService();
        var quoteService = new ShippingQuoteService();
        
        // Create and start the shipping quote application
        var app = new ShippingQuoteApplication(inputService, validationService, quoteService);
        app.Start();
    }
}

// Interface for handling user input
interface IInputService
{
    double GetNumericInput(string prompt);
}

// Interface for package validation
interface IValidationService
{
    bool ValidateWeight(double weight, out string error);
    bool ValidateDimensions(double width, double height, double length, out string error);
}

// Interface for quote calculation
interface IQuoteService
{
    double CalculateQuote(double weight, double width, double height, double length);
}

// Implementation of input service using console
class ConsoleInputService : IInputService
{
    public double GetNumericInput(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            try
            {
                return Convert.ToDouble(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }
}

// Implementation of validation service with business rules
class PackageValidationService : IValidationService
{
    private const double MaxWeight = 50;
    private const double MaxDimensions = 50;

    public bool ValidateWeight(double weight, out string error)
    {
        error = string.Empty;
        if (weight > MaxWeight)
        {
            error = "Package too heavy to be shipped via Package Express. Have a good day.";
            return false;
        }
        return true;
    }

    public bool ValidateDimensions(double width, double height, double length, out string error)
    {
        error = string.Empty;
        if (width + height + length > MaxDimensions)
        {
            error = "Package too big to be shipped via Package Express.";
            return false;
        }
        return true;
    }
}

// Implementation of quote calculation service
class ShippingQuoteService : IQuoteService
{
    public double CalculateQuote(double weight, double width, double height, double length)
    {
        return (width * height * length * weight) / 100;
    }
}

// Main application class that coordinates the shipping quote process
class ShippingQuoteApplication
{
    private readonly IInputService _inputService;
    private readonly IValidationService _validationService;
    private readonly IQuoteService _quoteService;

    public ShippingQuoteApplication(
        IInputService inputService,
        IValidationService validationService,
        IQuoteService quoteService)
    {
        _inputService = inputService;
        _validationService = validationService;
        _quoteService = quoteService;
    }

    public void Start()
    {
        // Display welcome message
        Console.WriteLine("Welcome to Package Express. Please follow the instructions below.");

        // Get and validate weight
        var weight = _inputService.GetNumericInput("Please enter the package weight:");
        if (!_validationService.ValidateWeight(weight, out string weightError))
        {
            Console.WriteLine(weightError);
            return;
        }

        // Get dimensions
        var width = _inputService.GetNumericInput("Please enter the package width:");
        var height = _inputService.GetNumericInput("Please enter the package height:");
        var length = _inputService.GetNumericInput("Please enter the package length:");

        // Validate dimensions
        if (!_validationService.ValidateDimensions(width, height, length, out string dimensionsError))
        {
            Console.WriteLine(dimensionsError);
            return;
        }

        // Calculate and display quote
        var quote = _quoteService.CalculateQuote(weight, width, height, length);
        Console.WriteLine($"Your estimated total for shipping this package is: ${quote:F2}");
        Console.WriteLine("Thank you!");
    }
}