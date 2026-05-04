---
title: "How to use the GravitationalField microservice?"
output: html_document
---

Typical Usage
===
1. Upload a new GravitationalFieldCalculationOrder using the `Post` web api method.
2. Call the `Get` method with the identifier of the uploaded GravitationalFieldCalculationOrder as argument. 
The return Json object contains the GravitationalFieldCalculationOrder description.
3. Optionally send a `Delete` request with the identifier of the GravitationalFieldCalculationOrder in order to delete the GravitationalFieldCalculationOrder if you do not 
want to keep the GravitationalFieldCalculationOrder uploaded on the microservice.


