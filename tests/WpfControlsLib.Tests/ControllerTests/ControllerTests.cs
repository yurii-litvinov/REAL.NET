namespace WpfControlsLib.Tests
{
    using System;
    using NUnit.Framework;
    using EditorPluginInterfaces;
    using WpfControlsLib.Controller;

    [TestFixture]
    public class ControllerTests
    {
        /// <summary>
        /// Helper class that allows to create commands on demand.
        /// </summary>
        private class AdHocCommand : ICommand
        {
            private Action @do;
            private Action undo;

            public AdHocCommand(Action @do, Action undo)
            {
                this.@do = @do;
                this.undo = undo;
            }

            public AdHocCommand(Action @do)
            {
                this.@do = @do;
            }

            public bool CanBeUndone => this.undo != null;

            public void Execute()
            {
                @do();
            }

            public void Undo()
            {
                undo();
            }
        }

        [Test]
        public void ControllerShouldExecuteGivenCommand()
        {
            var executed = false;
            var testCommand = new AdHocCommand(() => executed = true);
            var controller = new Controller();

            controller.Execute(testCommand);

            Assert.True(executed);
        }

        [Test]
        public void ControllerShouldAllowToUndoCommand()
        {
            var executed = false;
            var undone = false;
            var testCommand = new AdHocCommand(() => executed = true, () => undone = true);
            var controller = new Controller();

            controller.Execute(testCommand);

            Assert.True(executed);
            Assert.False(undone);

            controller.Undo();

            Assert.True(executed);
            Assert.True(undone);
        }

        [Test]
        public void ControllerShouldAllowToRedoCommand()
        {
            var executed = 0;
            var undone = false;
            var testCommand = new AdHocCommand(() => ++executed, () => undone = true);
            var controller = new Controller();

            controller.Execute(testCommand);

            Assert.AreEqual(1, executed);
            Assert.False(undone);

            controller.Undo();

            Assert.AreEqual(1, executed);
            Assert.True(undone);

            controller.Redo();

            Assert.AreEqual(2, executed);
        }

        [Test]
        public void ControllerShouldReportUndoRedoStatus()
        {
            var testCommand = new AdHocCommand(() => { }, () => { });
            var controller = new Controller();

            var undoAvailable = false;
            var redoAvailable = false;
            controller.UndoAvailabilityChanged += (_, args) => undoAvailable = args.IsAvailable;
            controller.RedoAvailabilityChanged += (_, args) => redoAvailable = args.IsAvailable;

            controller.Execute(testCommand);

            Assert.True(undoAvailable);
            Assert.False(redoAvailable);

            controller.Undo();

            Assert.False(undoAvailable);
            Assert.True(redoAvailable);

            controller.Redo();

            Assert.True(undoAvailable);
            Assert.False(redoAvailable);
        }

        [Test]
        public void ControllerShouldClearRedoStackOnNewCommand()
        {
            var testCommand = new AdHocCommand(() => { }, () => { });
            var controller = new Controller();

            var undoAvailable = false;
            var redoAvailable = false;
            controller.UndoAvailabilityChanged += (_, args) => undoAvailable = args.IsAvailable;
            controller.RedoAvailabilityChanged += (_, args) => redoAvailable = args.IsAvailable;

            controller.Execute(testCommand);
            controller.Undo();

            Assert.False(undoAvailable);
            Assert.True(redoAvailable);

            controller.Execute(testCommand);

            Assert.True(undoAvailable);
            Assert.False(redoAvailable);
        }
    }
}
