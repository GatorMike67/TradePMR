Issues encountered with TradePMR Web API exercise:


- Initially I had a Foreign Key constraint relating the AccountId of the Trade table back to the Id of the Accounts table. This created somewhat of a "circular reference" in the Entity classes generated due to the Account class having a Trades collection and the Trade class having an Account object in it. This resulted in a stack overflow as I was exercising some of the API calls. I removed the FK constraint in order to keep making progress.

 