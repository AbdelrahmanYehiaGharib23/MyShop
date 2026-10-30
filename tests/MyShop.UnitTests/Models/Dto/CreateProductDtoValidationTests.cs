using System.ComponentModel.DataAnnotations;
using MyShop.BLL.Models.Dto.ProductDto;

namespace MyShop.UnitTests.Models.Dto;

[Trait("Category", "Unit")]
public sealed class CreateProductDtoValidationTests
{
    [Fact]
    public void Validate_ReportsNamePriceAndCategoryErrorsForInvalidInput()
    {
        var dto = new CreateProductDto
        {
            Name = "ab",
            Price = 0,
            CategoryId = 0
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            dto,
            new ValidationContext(dto),
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, result =>
            result.MemberNames.Contains(nameof(CreateProductDto.Name)));
        Assert.Contains(validationResults, result =>
            result.MemberNames.Contains(nameof(CreateProductDto.Price)));
        Assert.Contains(validationResults, result =>
            result.MemberNames.Contains(nameof(CreateProductDto.CategoryId)));
    }

    [Fact]
    public void Validate_AcceptsValidProductInput()
    {
        var dto = new CreateProductDto
        {
            Name = "Leather bag",
            Price = 1250.50m,
            CategoryId = 1,
            Description = "A durable everyday bag."
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            dto,
            new ValidationContext(dto),
            validationResults,
            validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
}
