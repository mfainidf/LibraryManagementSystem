using System;
using System.IO;

namespace Library.Console
{
    public class ConsoleManager
    {
        private volatile bool _isActive = true;

        public void SetActive(bool value)
        {
            _isActive = value;
        }

        public void Clear()
        {
            try
            {
                if (_isActive)
                {
                    System.Console.Clear();
                }
            }
            catch (IOException)
            {
                // Ignora errori di I/O sulla console
            }
            catch (ObjectDisposedException)
            {
                // Ignora se la console è già stata disposta
            }
        }

        public void Write(string text)
        {
            try
            {
                if (_isActive)
                {
                    System.Console.Write(text);
                }
            }
            catch (IOException)
            {
                // Ignora errori di I/O sulla console
            }
            catch (ObjectDisposedException)
            {
                // Ignora se la console è già stata disposta
            }
        }

        public void WriteLine(string text = "")
        {
            try
            {
                if (_isActive)
                {
                    System.Console.WriteLine(text);
                }
            }
            catch (IOException)
            {
                // Ignora errori di I/O sulla console
            }
            catch (ObjectDisposedException)
            {
                // Ignora se la console è già stata disposta
            }
        }

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            try
            {
                if (_isActive)
                {
                    return System.Console.ReadKey(intercept);
                }
            }
            catch (IOException)
            {
                // Ignora errori di I/O sulla console
            }
            catch (ObjectDisposedException)
            {
                // Ignora se la console è già stata disposta
            }
            
            return new ConsoleKeyInfo('\0', ConsoleKey.Clear, false, false, false);
        }

        public string? ReadLine()
        {
            try
            {
                if (_isActive)
                {
                    return System.Console.ReadLine();
                }
            }
            catch (IOException)
            {
                // Ignora errori di I/O sulla console
            }
            catch (ObjectDisposedException)
            {
                // Ignora se la console è già stata disposta
            }
            
            return null;
        }
    }
}
