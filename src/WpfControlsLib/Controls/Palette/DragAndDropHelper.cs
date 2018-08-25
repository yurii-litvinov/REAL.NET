/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System.Runtime.InteropServices;
using System.Windows;

namespace WpfControlsLib.Controls.Palette
{
    /// <summary>
    /// Service class for performing drag-and-drop operation.
    /// </summary>
    internal static class DragAndDropHelper
    {
        public static void DoDragAndDrop(UIElement initiator, object data, string format, string image)
        {
            var dragData = new DataObject(format, data);

            var (window, handler) = CreateDragDropWindow(initiator, image);

            // Registering drag-and-drop feedback handler to show dragged element.
            initiator.GiveFeedback += handler;

            DragDrop.DoDragDrop(initiator, dragData, DragDropEffects.Copy);

            window?.Close();
            initiator.GiveFeedback -= handler;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public int X;
            public int Y;
        };

        private static (Window, GiveFeedbackEventHandler) CreateDragDropWindow(UIElement initiator, string image)
        {
            var scale = PresentationSource.FromVisual(initiator)?.CompositionTarget?.TransformToDevice.M11;
            var w32Mouse = new Win32Point();

            (int x, int y) GetAbsoluteCursorPos()
            {
                GetCursorPos(ref w32Mouse);

                if (scale.HasValue)
                {
                    w32Mouse.X = (int)(w32Mouse.X / scale.Value);
                    w32Mouse.Y = (int)(w32Mouse.Y / scale.Value);
                }

                return (w32Mouse.X, w32Mouse.Y);
            }

            var dragDropWindow = new DragAndDropFeedback {Image = image};

            void Handler(object sender, GiveFeedbackEventArgs args) 
                => (dragDropWindow.Left, dragDropWindow.Top) = GetAbsoluteCursorPos();

            // First run to init window coordinates.
            Handler(null, null);

            dragDropWindow.Show();

            return (dragDropWindow, Handler);
        }
    }
}
