# Checkout System Kata

## Overview
This project, implemented as a code kata for the second stage of an interview with the EAP provider Health Assured, creates a checkout system complete with pricing and special offer logic.

## Features Implemented
- Scan() and GetTotalPrice() method (as per the suggested interface) for adding items to basket, and retrieving their combined price including any applicable offers.
- InitialiseItems() method added to class to allow for pricing and offers to be set.
- Prices may be set per item SKU.
- Special offers may be set based on SKU and quantity.
- Detailed unit tests covering various scenarios, including some edge cases such as empty baskets, null or empty SKU's, and negative or zero pricings.

## Key Design Choices
- **Product SKU's**: 
  - The task indicates product SKU's will be given as letters of the alphabet. I have made the following assumptions:
    - SKU's are case sensitive. This will allow for a wider range of SKU's to be included. The main drawback of this choice would be that it may cause confusion if users were to mistype a SKU. I believe however, that if the main use case will be to scan products in via a barcode-esque system, this should not cause excessive frustration as compared to overly limiting the maximum number of products.
	- SKU's may also be set as punctuation or special character values. This again will allow a broader range of products to be included. It is worth noting that where this system to be connected to a SQL database, we may wish then disallow certain characters to avoid security issues such as SQL injection attacks.
	- Finally, SKU's may be set as multiple character codes (i.e. "AB", "HEN", etc) this again avoids unnecessarily restricting the number of products which the system may handle. Additionally it may allow for more identifiable SKU's to be selected to allow for ease of use by humans (i.e. "MLK" may be used to refer to milk).
- **Product Initialisation**
  - I have provided a method for initialising Prices. This allows for greater diversity in how the class library may be implemented into a wider system than if these prices were required during class construction.
  - However the drawback of this choice is that if prices were altered mid-checkout, it may lead to a case where scanned items cease to exist. To avoid this causing error I have implemented it such that any existing basket is cleared by the re-initialisation of prices.
  - I believe that clearing the basket is a more optimal solution to the problem than simply throwing error as it would provide for a more seamless experience should a sale need to be abandoned, allowing new pricing to be provided for the next transaction..
  - Should the specific implementation of the wider system require this to be altered such that mid-checkout price changes could occur, I would suggest further testing would require creating in order to ensure error is thrown (or the issue otherwise handled) in such cases where a scanned item ceases to exist as a product.
- **Negative Product Prices**
  - I have chosen to disallow negative pricing. The main reasoning being that negative pricings could become confusing for human users and/or allow for situations in which a customer may be able to receive a negative total price; which would require specific processing relavent to the wider system's implementation. Some points to consider where negative pricing may be applicable however could be:
    - **Product returns:** these could potentially handled by a separate SKU for returning said product, with a negative price attached. It is more likely however our shop would handle this on a separate system as there may be other considerations such as whether the refund is to cash, card, or store credit/item replacement.
    - **Gift Cards:** gift cards could be issued with a SKU set to a negative price allowing for their value to be deducted from a total. However again this is likely to be handled by a different system to both ensure that gift cards are not cash redeemable, and also allow for cases such as the partial redemption of a card, adding value to a card, or de-validating a card following its depletion.
  - In the event that the wider system did require handling negative prices, I would suggest looking at functionality to ensure the minimum total calculated is still Zero or positive; this would allow for gift cards or refunds to be used to over-cover the cost of a purchase, without them becoming cash-redeemable.
- **decimal verses integer Prices**
  - The task provides a suggested interface to return an integer total price, and furthermore all example prices given where integers. 
  - However, given that 'true' prices are in fact decimal values, I have built the system as such in order that in the future, the shop will be able to make use of decimal prices should they choose to do so.
  - In order to comply with the suggested interface I have cast the total price to an integer at the point of return which will in essence round any prices down to the nearest whole number.
  - I think that if kept, this functionality could be used as a USP for the shop and a potential marketing gimmick.
  - It is also worth considering the potential downsides too however, as finding unexpected discrepencies in the pricing of their shop may have a negative impact on our customer's trust in our systems (even if they are saving some pennies!).
- **Code Structure**
  - For a wider system's implementation I believe it would make sense to look to extracting some of the logic included in this project to a separate class to allow for sharing of the functionality.
  - In particular, the validation of pricing for Offers and Products could be extracted to a shared file (i.e. ProductValidator.cs) such that any other areas of the system which are required to perform similar validation could share the same code.
  - This would reduce code duplication and ensure uniformity of validation throughout the system.
  - For the purpose of this excercise however, I have aimed not to overengineer the solution and believe it's overall readability to be improved by not requiring the reader to jump between files for the sake of such a small number of lines.
  - Were this system to expand in the future I would strongly advise re-visiting or re-exploring the potential modularity which could be found within this existing code.
- **Testing Structure**
  - As per the code structure, I debated the validity of breaking the test class into separate files, each testing a specific method from the class.
  - I concluded however that it is more aligned with standard practice to have a single test class for the testing of the single checkout class.
  - In an effort to maintain some form of structure however, I have organized the testing of each method by utilizing code regions, making navigation easier for the reader.
  - These regions allow for readers to quickly navigate the code sections by searching for the "#region" keyword, or using the collapsable sections which the keyword provides to code editors such as Visual Studio.

