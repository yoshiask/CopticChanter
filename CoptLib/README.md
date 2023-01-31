# CoptLib
A .NET Standard 2.0 library made to handle scenarios specific to Coptic for Coptic Chanter.

# Feature overview
- CopticCalendar
    - Determine the current date in the Coptic calendar and era, including handling of the sunset transition time
    - Calculate the date(s) a fast or feast occurs on for a given Coptic year
    - Tools for automatically computing the next occurrence of a fast or feast
- CopticInterpreter
    - Convert between Coptic fonts, including Coptic1, CS Avva Shenouda [Tasbeha.org], and Coptic Unicode
    - Perform phonetic analysis on Coptic and Greek text, which includes guessing word origin and pronunciation
    - Transliterate Coptic text to English or Arabic letters using the Greco-Bohairic pronunciation rules
- Scripting
    - Powerful inline text commands for reducing duplicated text and transforming content at runtime
    - Run C# scripts embedded in Documents for complex logic
- IO
    - Read/write/transform Document XML files
    - Read and create Document Sets