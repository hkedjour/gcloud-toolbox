#region License
// Copyright (c) 2019 Hichem Kedjour
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System.Windows.Input;
using GCloudToolbox.Core;
using GCloudToolbox.Core.Wpf.Commands;

namespace GCloudToolbox.Shell.ViewModels
{
    /// <summary>
    /// An entry in the main application menu.
    /// </summary>
    public class ToolMenuEntry
    {
        public ToolMenuEntry(ITool tool)
        {
            Header = tool.Header;
            Command = new AsyncCommand<object>(o => tool.LaunchAsync(), o => true);
        }

        /// <summary>
        /// The menu item header.
        /// </summary>
        public IHeader Header { get; }

        /// <summary>
        /// The menu item command.
        /// </summary>
        public ICommand Command { get; }
    }
}