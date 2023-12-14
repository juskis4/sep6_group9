*** Settings ***

Library    SeleniumLibrary


*** Variables ***
${chrome_driver_path}    C:/Users/vaiti/Desktop/MovieCentral/Driver/chromedriver.exe    #THIS PATH NEEDS TO BE ADAPTED DEPENDING ON WHO IS RUNNING THE TESTS


*** Keywords ***
Open Chrome
    ${device metrics}=    Create Dictionary    width=${1366}    height=${768}    pixelRatio=${1.0}
    ${chrome options}=    Evaluate    sys.modules['selenium.webdriver'].ChromeOptions()    sys, selenium.webdriver
    Call Method    ${chrome options}    add_argument    --start-maximized
    Create Webdriver    Chrome    chrome_options=${chrome options}    executable_path=${chrome_driver_path}


*** Test Cases ***

Open Web Application

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Close Browser

Open Register Page

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Click Element    xpath=/html/body/div/main/div/div/a    #CLICK ON THE SIGN UP BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/div/h3    #WAIT UNTIL THE REGISTER TITLE APPEARS
    Close Browser

Login to an account

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Close Browser

Go to "Stars" page

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Click Element    xpath=//*[@id="navbarNavAltMarkup"]/div[1]/a[2]    #CLICK ON STARTS PAGE
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/h1    10s    #WAIT UNTIL THE PAGE IS LOADED AND CHECK IF THE TITLE IS CORRECT
    Close Browser

Go to "Directors" page

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Click Element    xpath=//*[@id="navbarNavAltMarkup"]/div[1]/a[3]    #CLICK ON DIRECTORS PAGE
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/h1    10s    #WAIT UNTIL THE PAGE IS LOADED AND CHECK IF THE TITLE IS CORRECT
    Close Browser

Go to "Profile" page

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Click Element    xpath=//*[@id="navbarNavAltMarkup"]/div[1]/a[4]    #CLICK ON THE PROFILE PAGE
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/h2    10s    #WAIT UNTIL THE PAGE IS LOADED AND CHECK IF THE TITLE IS CORRECT
    Close Browser

Search for a movie

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Input Text    xpath=//*[@id="navbarNavAltMarkup"]/div[2]/form[1]/div/input    Cars    #SEARCH FOR A MOVIE
    Sleep    2s
    Click Element    xpath=//*[@id="navbarNavAltMarkup"]/div[2]/form[1]/div/div/button    #CLICK ON THE SEARCH BUTTON
    Sleep    5s
    Close Browser

Filter the movies by a release year

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Click Element    xpath=/html/body/div/main/form/div/div[1]/select    #CLICK ON THE YEAR DROPDOWN
    Sleep    2s
    Click Element    xpath=/html/body/div/main/form/div/div[1]/select/option[4]    #CLICK ON THE YEAR 1972
    Sleep    10s
    Close Browser

Logout from the account

    Open Chrome
    Go To    https://sep6-group9-746yuurmra-uc.a.run.app/    #THIS CAN BE CHANGED TO ANY OTHER URL (DEPENDING IF THE WEB APP IS HOSTED SOMEWHERE)
    Sleep    2s    #WAIT TO SEE IF THE TEST OPENED THE CORRECT PAGE
    Input Text    xpath=//*[@id="Username"]    felixtest    #USERNAME TEXT FIELD
    Sleep    2s
    Input Text    xpath=//*[@id="Password"]    testtest    #PASSWORD TEXT FIELD
    Sleep    2s
    Click Element    xpath=/html/body/div/main/div/form/button    #CLICK THE LOGIN BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/form/div/div[2]/label    10s    #WAIT UNTIL TH LOGIN IS SUCCESSFUL AND THE MAIN PAGE IS DISPLAYED
    Click Element    xpath=//*[@id="navbarNavAltMarkup"]/div[2]/form[2]/button    #CLICK ON THE LOGOUT BUTTON
    Sleep    2s
    Wait Until Element Is Visible    xpath=/html/body/div/main/div/h3    #WAIT TO SEE IF THE LOGIN PAGE IS LOADED
    Close Browser