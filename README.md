# **ClickLoop**

ClickLoop is a Windows-based automation tool designed to interact with processes and simulate key inputs in a loop. It is ideal for repetitive tasks, such as sending automated messages or performing repetitive actions in a specific application.

---

## **Features**
- **Process Interaction**: Select and interact with running processes.
- **Key Simulation**: Simulate key inputs, including special keys like `{ENTER}` and `{BS}`.
- **Customizable Loop**: Configure the interval and typing speed for the loop.
- **Process Monitoring**: Automatically stop the loop if the target process is closed.
- **Automatic Text Formatting**: Automatically replaces spaces with `{BS}` and newlines with `{ENTER}` for seamless key simulation.
- **User-Friendly UI**: Simple and intuitive interface for configuration and control.

---

## **Installation**

### **Prerequisites**
- **.NET 9 Runtime**: Ensure that the .NET 9 runtime is installed on your system. You can download it from the [official .NET website](https://dotnet.microsoft.com/).

### **Steps**
1. Clone the repository:
~~~~
    git clone https://github.com/your-repo/ClickLoop.git
~~~~
2. Open the solution in **Visual Studio 2022**.
3. Build the project to restore dependencies and compile the application.
4. Run the application by pressing `F5` or by navigating to the `bin/Release` or `bin/Debug` folder and executing the `.exe` file.

---

## **Usage**

### **How to Use**
1. **Select a Process**:
   - Use the dropdown to select a running process from the list.
   - If the desired process is not listed, click the "Refresh" button.
2. **Configure Loop Settings**:
   - Set the loop interval (in seconds) using the first numeric input.
   - Set the typing speed (in seconds per key) using the second numeric input.
3. **Enter Text**:
   - Type the text to be sent in the input box.
   - **Automatic Formatting**: 
     - Spaces (` `) are automatically replaced with `{BS}`.
     - Newlines (`\r\n`) are automatically replaced with `{ENTER}`.
4. **Start the Loop**:
   - Click the "Start" button to begin the loop.
   - The application will bring the selected process to the foreground and simulate the key inputs.
5. **Stop the Loop**:
   - Click the "Cancel" button to stop the loop.

### **Additional Features**
- **Support**: Access the tutorial and tips via the "Support" menu.
- **Version Check**: Check for updates via the "Version" menu.
- **Contact**: Use the "Contact Me" menu to send feedback or inquiries.

---

## **Contributing**
Contributions are welcome! Feel free to fork the repository and submit a pull request.

---

## **License**
This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## **Acknowledgments**
Special thanks to all contributors and users for their feedback and support.

---

