@Counter
Feature: Counter

Scenario: Counter page is displayed
    Given a user visits the counter page
    Then the counter heading is "Counter"

Scenario: Counter increments when button is clicked
    Given a user visits the counter page
    Then the current count is 0
    When the user clicks increment
    Then the current count is 1
