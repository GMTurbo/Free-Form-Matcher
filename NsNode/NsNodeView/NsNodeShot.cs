using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Standard;
using devDept.Eyeshot.Geometry;
using devDept.Eyeshot.Labels;
using NsNodes;

namespace NsNodeView
{
	public partial class NsNodeShot : UserControl, INodeView, INodeCtrl, ITracker
	{
		public NsNodeShot()
		{
			InitializeComponent();
			
			Viewport.Grid = new devDept.Eyeshot.Grid(new devDept.Eyeshot.Point2D(-2000, -2000), new devDept.Eyeshot.Point2D(10000, 10000), 2000, System.Drawing.Color.Black, System.Drawing.Color.White, true, true);
			Viewport.UnitsMode = devDept.Eyeshot.viewportUnitsType.Millimeters;
			Viewport.DisplayMode = viewportDisplayType.Shaded;
			Viewport.ShowFps = false;
			Viewport.Edge.ColorMode = edgeColorType.EntityColor;
			Viewport.AllowDrop = false;
			Viewport.Grid.Visible = true;

			CreateCameraStrip();
			SetCamera(viewType.Trimetric, cameraProjectionType.Perspective);

          }

		public ViewportProfessional Viewport
		{
			get { return viewport; }
		}

		EntityList Entities
		{
			get { return Viewport.Entities; }
		}

		public static System.Drawing.Font LABELFONT = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace, 10f, System.Drawing.FontStyle.Bold);
		List<LabelBase> Labels
		{
			get { return Viewport.Labels; }
		}

		public void Clear()
		{
			Viewport.Clear();
		}

          public Point3D ScreenToWorld(System.Drawing.Point mousePos, Plane plane)
          {
               return viewport.ScreenToWorld(mousePos, plane);
          }

		#region INodeView Members

		public void OnNodeUpdated(object sender, NodeEventArgs e)
		{
			if (e.Node == null)
				Clear();
			else
				UpdateNode(e.Node);

			Viewport.Refresh();
		}
		public void OnNodeRemoved(object sender, NodeEventArgs e)
		{
			RemoveNode(e.Node);
			Viewport.Refresh();
		}

		void RemoveNode(NsNode node)
		{
			if (node == null)
				return;
			IEntityGroup ent;
			if (node is IEntityGroup)
			{
				ent = node as IEntityGroup;
				RemoveEntities(ent);
			}
			foreach (IAttribute atr in node.Attributes)
			{
				if (atr is IEntityGroup)
				{
					ent = atr as IEntityGroup;
					RemoveEntities(ent);
				}
			}
			foreach (NsNode n in node)
				RemoveNode(n);
		}
		private void RemoveEntities(IEntityGroup ent)
		{
			Entity[] ents = GetEntities(ent);
			if (ents != null)
				foreach (Entity e in ents)
					Entities.Remove(e);
				Labels.Remove(ent.devpDeptLabel);
		
		}

		void UpdateNode(NsNode node)
		{
			IEntityGroup ent;
			if (node is IEntityGroup)
			{
				ent = node as IEntityGroup;
				UpdateEntities(ent);
			}
			foreach (IAttribute atr in node.Attributes)
			{
				if (atr is IEntityGroup)
				{
					ent = atr as IEntityGroup;
					UpdateEntities(ent);
				}
			}
			foreach (NsNode n in node)
				UpdateNode(n);
		}
		private void UpdateEntities(IEntityGroup ent)
		{
			Entity[] ents = GetEntities(ent);
			bool reselect = false;
			if (ents != null)
                foreach (Entity e in ents)
                {
                    reselect = e.Selected;
                    Entities.Remove(e);
                    //if (ent.devpDeptLabel != null)
                    Labels.Remove(ent.devpDeptLabel);
                    if (ent is NsNode)
                    {
                        if ((ent as NsNodes.NsNode).Label.Contains("Lifter") && ent.devpDeptLabel == null)
                            Labels.Clear();
                    }
                    else
                        continue;
                }

			Entity[] ee = ent.Entity;

			if(ee != null)
				foreach(Entity e in ee)
					if (e != null)
					{
						Entities.Add(e);
						e.Selected = reselect;
						if(ent.devpDeptLabel != null)
							Labels.Add(ent.devpDeptLabel);
					}
		}

		Entity[] GetEntities(IEntityGroup ent)
		{
			if (Entities == null)
				return null;
			List<Entity> ents = new List<Entity>();
			foreach (Entity e in Entities)
				if (e.EntityData == ent)
					ents.Add(e);
			//System.Diagnostics.Debug.Assert(ents.Count <= 1, "Should not have more than 1 entity for each IEntityGroup");
			if (ents.Count > 0)
				return ents.ToArray();
			else return null;
		}

		LabelBase[] GetLabels(IEntityGroup ent)
		{
			if (Entities == null)
				return null;
			List<LabelBase> ents = new List<LabelBase>();
			foreach (LabelBase e in Labels)
					ents.Add(e);
			//System.Diagnostics.Debug.Assert(ents.Count <= 1, "Should not have more than 1 entity for each IEntityGroup");
			if (ents.Count > 0)
				return ents.ToArray();
			else return null;
		}

		public void Attach(INodeCtrl ictrl)
		{
			ictrl.SelectionChanged += OnNodeUpdated;
		}

		#endregion

		#region INodeCtrl Members

		public event NsNodeEventHandler SelectionChanged;

		#endregion

		#region ITracker Members

		public bool ClearTrackers()
		{
			bool ret = false;
			if (TrackerDetach != null)
			{
				TrackerDetach(this, new NodeTrackerEventArgs());
				ret = true;
			}

			TrackerDown = null;
			TrackerDrag = null;
			TrackerUp = null;
			TrackerDetach = null;

			return ret;
		}

		public event NsNodeTrackerEventHandler TrackerDown;

		public event NsNodeTrackerEventHandler TrackerDrag;

		public event NsNodeTrackerEventHandler TrackerUp;

		public event NsNodeTrackerEventHandler TrackerDetach;

		public event NsNodeEventHandler NodeSelected;

		private void viewport_KeyUp(object sender, KeyEventArgs e)
		{
               switch (e.KeyCode)
               {
                    case Keys.Escape:
                         List<NsNode> selnodes;
                         if (!ClearTrackers()) //no trackers to clear
                              if (GetSelection(out selnodes).Count > 0)
                                   Viewport.ClearSelection();
                              else
                                   Viewport.ActionMode = viewportActionType.None;
                         Viewport.Refresh();
                         break;

                    case Keys.Z:
                         SetCamera(viewType.Top);
                         break;

                    case Keys.X:
                         SetCamera(viewType.Right);
                         break;

                    case Keys.C:
                         SetCamera(viewType.Rear);
                         break;

                    case Keys.V:
                         SetCamera(viewType.Left);
                         break;

                    case Keys.B:
                         SetCamera(viewType.Front);
                         break;

                    case Keys.D1:
                         Viewport.ActionMode = viewportActionType.SelectByPick;
                         break;

                    case Keys.D2:
                         Viewport.ActionMode = viewportActionType.SelectByBox;
                         break;

               }
		}

		#endregion

		#region Camera

		Camera Camera
		{
			get { return Viewport.Camera; }
		}
		viewType m_viewtype;

		public void SetCamera(cameraProjectionType type)
		{
			SetCamera(m_viewtype, type);
		}
		public void SetCamera(viewType type)
		{
			SetCamera(type, Camera.ProjectionMode);
		}
		public void SetCamera(viewType type, cameraProjectionType proj)
		{
			Camera.ProjectionMode = proj;
			m_viewtype = type;
			Viewport.SetCameraView(m_viewtype);

			if (Entities != null && Entities.Count > 0)
				ZoomFit();
			Refresh();
		}
		public void ResetCamera()
		{
			SetCamera(m_viewtype, Camera.ProjectionMode);
			ZoomFit();
		}
		public void ZoomFit()
		{
			Viewport.ZoomFit();
			Viewport.Invalidate();
			viewport.Refresh();
		}

		#region Strip

		ToolStripMenuItem CameraStrip
		{
			get
			{
				if (m_camerastrip == null)
					CreateCameraStrip();
				return m_camerastrip;
			}
			set { m_camerastrip = value; }
		}
		ToolStripMenuItem m_camerastrip;

		void CreateCameraStrip()
		{

			m_camerastrip = new System.Windows.Forms.ToolStripMenuItem("Camera", NsNodeView.Properties.Resources.Binoculars);
			foreach (int value in System.Enum.GetValues(typeof(viewType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(viewType), value));
				ts.Tag = (viewType)value;
				m_camerastrip.DropDownItems.Add(ts);
			}
			m_camerastrip.DropDownItems.Add("-");
			foreach (int value in System.Enum.GetValues(typeof(cameraProjectionType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(cameraProjectionType), value));
				ts.Tag = (cameraProjectionType)value;
				m_camerastrip.DropDownItems.Add(ts);
			}
			m_camerastrip.DropDownItems.Add("-");
			foreach (int value in System.Enum.GetValues(typeof(backgroundStyleType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(backgroundStyleType), value));
				ts.Tag = (backgroundStyleType)value;
				if (ts.Tag.ToString() != "Image")
					m_camerastrip.DropDownItems.Add(ts);
			}
			m_camerastrip.DropDownItems.Add("-");
			foreach (int value in System.Enum.GetValues(typeof(viewportDisplayType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(viewportDisplayType), value));
				ts.Tag = (viewportDisplayType)value;
				m_camerastrip.DropDownItems.Add(ts);
			}

			m_camerastrip.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(m_camerastrip_DropDownItemClicked);

			//create a default popup menu with the camera options
			if (this.ContextMenuStrip == null)
			{
				ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(); // this fires OnContextMenuChanged event
			}
			this.ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);
		}

		protected override void OnContextMenuStripChanged(EventArgs e)
		{
			base.OnContextMenuStripChanged(e);

			if (ContextMenuStrip != null && !ContextMenuStrip.Items.Contains(m_camerastrip))
			{
				ContextMenuStrip.AllowMerge = true;
				if (ContextMenuStrip.Items.Count > 0)
					ContextMenuStrip.Items.Add("-"); // add a seperator
				ContextMenuStrip.Items.Add(CameraStrip);
			}
			if (ContextMenuStrip != null && !ContextMenuStrip.Items.ContainsKey("Mouse Mode"))
			{
				ToolStripMenuItem mouse = new ToolStripMenuItem("Mouse Mode");
				foreach (int value in System.Enum.GetValues(typeof(viewportActionType)))
				{
					ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(viewportActionType), value));
					ts.Tag = (viewportActionType)value;
					mouse.DropDownItems.Add(ts);
				}
				if (ContextMenuStrip.Items.Count > 0)
					ContextMenuStrip.Items.Add("-");
				ContextMenuStrip.Items.Add(mouse);
				mouse.DropDownItemClicked += new ToolStripItemClickedEventHandler(mouse_DropDownItemClicked);
			}
			if (ContextMenuStrip != null && !ContextMenuStrip.Items.Contains(m_printstrip))
			{
				ContextMenuStrip.AllowMerge = true;
				if (ContextMenuStrip.Items.Count > 0)
					ContextMenuStrip.Items.Add("-"); // add a seperator
				ContextMenuStrip.Items.Add(PrintStrip);
			}
		}
		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//set checkmarks
			viewType vt;
			cameraProjectionType cp;
			ToolStripMenuItem ts;
			viewportDisplayType vd;
			backgroundStyleType bs;
			foreach (ToolStripItem item in CameraStrip.DropDownItems)
			{
				if (item is ToolStripMenuItem)
				{
					ts = (ToolStripMenuItem)item;
					if (ts.Tag is viewType)
					{
						vt = (viewType)ts.Tag;
						ts.Checked = vt == m_viewtype;
					}
					else if (ts.Tag is cameraProjectionType)
					{
						cp = (cameraProjectionType)ts.Tag;
						ts.Checked = cp == Camera.ProjectionMode;
					}
					else if (ts.Tag is viewportDisplayType)
					{
						vd = (viewportDisplayType)ts.Tag;
						ts.Checked = vd == Viewport.DisplayMode;
					}
					else if (ts.Tag is backgroundStyleType)
					{
						bs = (backgroundStyleType)ts.Tag;
						ts.Checked = bs == Viewport.Background.StyleMode;
					}
				}
			}
		}
		void m_camerastrip_DropDownItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
		{
			try
			{
				if (e.ClickedItem.Tag is viewType)
				{
					viewType v = (viewType)e.ClickedItem.Tag;
					SetCamera(v, Camera.ProjectionMode);
				}
				else if (e.ClickedItem.Tag is cameraProjectionType)
				{
					cameraProjectionType v = (cameraProjectionType)e.ClickedItem.Tag;
					this.Camera.ProjectionMode = v;
					Refresh();
				}
				else if (e.ClickedItem.Tag is viewportDisplayType)
				{
					viewportDisplayType v = (viewportDisplayType)e.ClickedItem.Tag;
					Viewport.DisplayMode = v;
					Refresh();
				}
				else if (e.ClickedItem.Tag is backgroundStyleType)
				{
					backgroundStyleType v = (backgroundStyleType)e.ClickedItem.Tag;
					Viewport.Background.StyleMode = v;
					Refresh();
				}
			}
			catch { }
		}

		#endregion
		#endregion

		#region Printing

		ToolStripMenuItem PrintStrip
		{
			get
			{
				if (m_printstrip == null)
					CreatePrintStrip();
				return m_printstrip;
			}
			set { m_printstrip = value; }
		}
		ToolStripMenuItem m_printstrip;

		void CreatePrintStrip()
		{
			m_printstrip = new System.Windows.Forms.ToolStripMenuItem("Print");

			ToolStripMenuItem Save = new System.Windows.Forms.ToolStripMenuItem("Save");
			Save.Click += new EventHandler(Save_Click);
			Save.Tag = "save";
			m_printstrip.DropDownItems.Add(Save);

			ToolStripMenuItem copy = new ToolStripMenuItem("Copy");
			copy.Click += new EventHandler(Copy_Click);
			copy.Tag = "copy";
			m_printstrip.DropDownItems.Add(copy);

			ToolStripMenuItem Print = new System.Windows.Forms.ToolStripMenuItem("Print");
			Print.Click += new EventHandler(Print_Click);
			Print.Tag = "print";
			m_printstrip.DropDownItems.Add(Print);

			ToolStripMenuItem PrintSet = new System.Windows.Forms.ToolStripMenuItem("Page Setup");
			PrintSet.Click += new EventHandler(PrintSet_Click);
			PrintSet.Tag = "pageset";
			m_printstrip.DropDownItems.Add(PrintSet);

		}
		
		void Save_Click(object sender, EventArgs e)
		{
			SaveFileDialog mySaveFileDialog = new SaveFileDialog();

			mySaveFileDialog.InitialDirectory = ".";
			mySaveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|" +
				"Portable Network Graphics (*.png)|*.png|" +
				"Windows metafile (*.wmf)|*.wmf|" +
				"Enhanced Windows Metafile (*.emf)|*.emf|" +
				"Joint Photographic Experts Group (*.jpeg) |*.jpeg";

			mySaveFileDialog.FilterIndex = 2;
			mySaveFileDialog.RestoreDirectory = true;

			if (mySaveFileDialog.ShowDialog() == DialogResult.OK)
			{

				switch (mySaveFileDialog.FilterIndex)
				{

					case 1: Viewport.RenderToFile(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
						break;
					case 2: Viewport.RenderToFile(3, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
						break;
					case 3: Viewport.RenderToFile(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Wmf);
						break;
					case 4: Viewport.RenderToFile(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Emf);
						break;
					case 5: Viewport.RenderToFile(4, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
				}

			}
		}
		void Copy_Click(object sender, EventArgs e)
		{
			Viewport.CopyToClipboard(1);
		}
		void Print_Click(object sender, EventArgs e)
		{
			backgroundStyleType prev = Viewport.Background.StyleMode;
			Viewport.Background.StyleMode = backgroundStyleType.None;
			Viewport.ZoomFit();
			Viewport.PrintResolution = 1000;
			Viewport.PrintPreview(new Size(700, 700));
			Viewport.Background.StyleMode = prev;
		}
		void PrintSet_Click(object sender, EventArgs e)
		{
			Viewport.PageSetup();
		}

		#endregion

		#region Mouse
		Point3D prev = Point3D.MinValue;

		private void viewport_DoubleClick(object sender, EventArgs e)
		{
			ResetCamera();
		}

          public event MouseEventHandler OnMouseDown;
          public event MouseEventHandler OnMouseMove;
          public event MouseEventHandler OnMouseUp;

		private void viewport_MouseDown(object sender, MouseEventArgs e)
		{
			List<NsNode> nodes;
			List<Entity> selected = GetSelection(out nodes);

			foreach (NsNode n in nodes)
			{
				AttachDragNode(n);//attach all selected nodes to recieve drag event
				if (n is ITrackable)
					(n as ITrackable).AttachTracker(this);
			}

			prev = GetWorld(e.Location);

			if (e.Button == MouseButtons.Left && TrackerDown != null) //notify trackers
				TrackerDown(this, new NodeTrackerEventArgs(prev.X, prev.Y, prev.Z));
			//else if (e.Button == MouseButtons.Right)
			//     ClearTrackers();

               if (OnMouseDown != null)
                    OnMouseDown(sender, e);
		}

		private void viewport_MouseMove(object sender, MouseEventArgs e)
		{
			Point3D pnt = GetWorld(e.Location);
			if (e.Button == MouseButtons.Left)
			{
				if (TrackerDrag != null)
					//TrackerDrag(this, new NodeTrackerEventArgs(pnt.X, pnt.Y, pnt.Z));
					TrackerDrag(this, new NodeTrackerEventArgs(new double[3] { prev.X, prev.Y, prev.Z }, new double[3] { pnt.X, pnt.Y, pnt.Z }));

				if (pnt != Point3D.MinValue && prev != Point3D.MinValue && pnt != prev)
				{
					if (RaiseDragNode(prev, pnt))
					{//send drag node event
						
						Entities.Regen();
						Viewport.Refresh();
					}
					prev = pnt;
				}
			}

               if (OnMouseMove != null)
                    OnMouseMove(sender, e);
		}

		private void viewport_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				List<NsNode> nodes;
				List<Entity> selected = GetSelection(out nodes);

				if (SelectionChanged != null)
				{
					if (nodes.Count == 1)
						SelectionChanged(this, new NodeEventArgs(nodes[0]));
					else if( nodes.Count > 0 )
						SelectionChanged(this, new NodeEventArgs(nodes.ToArray()));
				}
				ClearDragNode();
				Point3D pnt = GetWorld(e.Location);
				if (e.Button == MouseButtons.Left && TrackerUp != null)
					TrackerUp(this, new NodeTrackerEventArgs(pnt.X, pnt.Y, pnt.Z));
			}

               if (OnMouseUp != null)
                    OnMouseUp(sender, e);
		}

		Point3D GetWorld(System.Drawing.Point mouse)
		{
			Point3D dir = (Viewport.Camera.Location - Viewport.Camera.Target);
			Vector3D N = new Vector3D(dir.X, dir.Y, dir.Z);
			Plane p = new Plane(Point3D.Origin, N);
			Point3D world;
			world = Viewport.ScreenToWorld(mouse, p);
			return world;
		}

		List<Entity> GetSelection(out List<NsNode> nodes)
		{
			List<Entity> selected = new List<Entity>();
			nodes = new List<NsNode>();
			foreach (Entity ent in Entities)
				if (ent.Selectable && ent.Selected)
				{
					nodes.Add(ent.EntityData as NsNode);
					selected.Add(ent);
				}
			//if(nodes.Count > 0)
			//    nodes.Last().OnNodeSelected(this, new NodeEventArgs(nodes.ToArray()));
			return selected;
		}

		List<NsNode> GetNodeList(IList<Entity> entities)
		{
			List<NsNode> nodes = new List<NsNode>(entities.Count);
			foreach (Entity e in entities)
			{
				NsNode n = GetNode(e);
				if (n != null && !nodes.Contains(n))//ensure unique
					nodes.Add(n);
			}
			return nodes;
		}

		NsNode GetNode(Entity e)
		{
			if (e.EntityData == null)
				return null;
			else if (e.EntityData is NsNode)
				return e.EntityData as NsNode;
			else if (e.EntityData is IAttribute)
				return (e.EntityData as IAttribute).Parent;
			else
				return null;
		}

		event NsNodeTrackerEventHandler DragNode;

		void AttachDragNode(NsNode node)
		{
			DragNode += node.OnDrag;
		}

		bool RaiseDragNode(Point3D start, Point3D end)
		{
			if (DragNode != null)
			{
				DragNode(this, new NodeTrackerEventArgs(new double[3] { start.X, start.Y, start.Z }, new double[3] { end.X, end.Y, end.Z }));
				return true;
			}
			return false;
		}

		void ClearDragNode()
		{
			if (DragNode != null)
				DragNode(this, new NodeTrackerEventArgs());
			prev = Point3D.MinValue;
			DragNode = null;
		}

		#region Strip

		void mouse_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag is viewportActionType)
			{
				viewportActionType v = (viewportActionType)e.ClickedItem.Tag;
				Viewport.ActionMode = v;
			}
		}

		#endregion

		#endregion

		#region INodeView Members

		public void OnNodeSelected(object sender, NodeEventArgs e)
		{
			if (NodeSelected != null)
				NodeSelected(sender, e);
		}

		#endregion

		#region INodeCtrl Members

		public event NsNodeEventHandler NodeCopied;

		#endregion
	}
}
