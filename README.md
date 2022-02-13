# linkedin_reach_code_sample_stripe

This repository addresses the implementation of Stripe Api in the Welrus application.

When a user submits their payment information, we take in the user information such as their credit card and send a request to Stripe server to receive back a customer Id and token Id.

After we've received the customer Id from Stripe, we can then create a charge request to the Stripe server again.

If the payment is processed successfully, we can store the response object in the database.

Upon completion of the payment, the requested appointment event will be confirmed under the user. 
