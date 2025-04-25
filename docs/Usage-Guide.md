# Usage Guide

This library enables the use of the Aira Unified chat and smart uploader, serving as an alternative admin user interface optimized for smartphones.

## Accessing the Aira Unified PWA (Progressive Web App)

After successfully configuring the Aira Unified application in the Xperience admin UI and assigning the required roles to the appropriate users, as detailed in the [README](../README.md), those users can access the application on their smartphones.

The configured URL will be relative to the base URL of the application. For example, if your base URL is `https://localhost:34886` and you have set the `Relative Path Base` to `/aira`, the Aira application will be available at `https://localhost:34886/aira/signin`.

After visiting that URL on a smartphone, users will see an installation dialog explaining the steps required to install the app as a PWA on their home screen. The process varies between smartphone operating systems. After completing one of the steps below, the application will appear on the [home screen](#aira-unified-on-home-screen).

1. **Android users**  
   Android supports PWA installation via a button. Therefore, Android users will be presented with an option to install the application. Once confirmed, the app will be added to their home screen.

   ![Android installation](/images/AiraUnifiedInstallationDialogAndroid.png)

2. **iOS users**  
   iOS does not support direct installation via a button. Instead, users will be guided on how to add the PWA to their home screen. This involves tapping the share button at the bottom of the Safari browser and selecting "Add to Home Screen."

   ![iOS installation](/images/AiraUnifiedInstallationDialogIOS.jpg)  
   ![Add to home screen](/images/AiraUnifiedAddToHomeScreen.jpg)  
   ![Create the PWA](/images/AiraUnifiedCreatePWA.jpg)  

### Aira Unified on Home Screen
![Aira Unified on home screen](/images/AiraUnifiedOnHomeScreen.jpg)

## Using the Chatbot

### Sign-in Process

After opening the PWA, if a user is not signed in yet, they will see the sign-in screen. Users with appropriate permissions can sign in by:

1. Clicking the **Continue with XbyK** button
2. Entering their Xperience admin credentials
3. Completing any required authentication steps (e.g., 2FA if enabled)
4. Closing the administration interface after successful sign-in

![Sign in](/images/AiraUnifiedSignIn.jpg)

Aira Unified remembers the signed-in user and the PWA will not require repeating the sign-in process unless:
- The user explicitly signs out
- The session expires
- The browser cache is cleared

### Chat Features

After signing in, users will be redirected to the chat page where they can:

1. **Ask Questions**: Type any question about Xperience functionality
2. **Get Insights**: Ask about content, marketing, or email insights
3. **View History**: Access previous conversations
4. **Use Quick Prompts**: Select from suggested questions

Example queries:
- "How do I create a new page?"
- "Show me content insights"
- "What are my recent marketing activities?"
- "Help me with email campaigns"

![Chat](/images/AiraUnifiedChat.jpg)

## Using the Smart Uploader

Admin application users with a role that includes **Create** permission for the Aira Unified application can access the Smart Uploader.

It can be opened by:
1. Tapping the menu button
2. Navigating to the **Smart Upload** page

![Menu](/images/AiraUnifiedNavigation.jpg)  
![Smart uploader](/images/SmartAssetUploader.jpg)

The Smart Uploader allows you to:
- Upload multiple files at once
- Automatically categorize content
- Add metadata to uploaded files
- Preview content before publishing

## Troubleshooting

### Common Issues

1. **Sign-in Problems**
   - Ensure you have the correct permissions
   - Check your internet connection
   - Verify your credentials are correct

2. **Chat Not Responding**
   - Check your internet connection
   - Verify the API key is correctly configured
   - Ensure the service is running

3. **Upload Issues**
   - Check file size limits
   - Verify file types are supported
   - Ensure you have sufficient permissions

## Upgrades and Uninstalling

See [Upgrades](Upgrades.md)

## Configuration options

See [Configuration options](Configuration-Options.md)

## Mock Messages Guide

See [Mock messages guide](Mock-Messages-Guide.md)