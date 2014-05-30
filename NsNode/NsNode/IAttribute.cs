using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NsNodes;

namespace NsNodes
{
	/// <summary>
	/// Required interface to be stored in an NsNode
	/// </summary>
	public interface IAttribute
	{
		/// <summary>
		/// The label of this attribute, must be unique for its parent node
		/// </summary>
		string Label { get; }
		/// <summary>
		/// The Value of this attribute
		/// </summary>
		object Value { get; set; }
		/// <summary>
		/// The parent node that contains this attribute
		/// </summary>
		NsNode Parent { get; }
		/// <summary>
		/// Determines if the attribute matches the query string, this should be only the final node of the query string
		/// </summary>
		/// <param name="query">the query string</param>
		/// <returns></returns>
		bool Query(string query);
		/// <summary>
		/// The Type of this attribute, used for serialization
		/// </summary>
		string Type { get; }

		/// <summary>
		/// serializes this attribute to xml format and returns the element, it must be manually added to the doc afterwards
		/// </summary>
		/// <param name="doc">the document used to create the xml</param>
		/// <returns>a new XmlElement representing this attribute</returns>
		XmlElement ToXml(XmlDocument doc);
		/// <summary>
		/// deserializes this attribute from xml format
		/// </summary>
		/// <param name="xml">the XmlNode representing this attribute</param>
		/// <returns>true if successul, false if improper format</returns>
		bool FromXml(XmlNode xml);

		/// <summary>
		/// Removes the attribute from its parent node
		/// </summary>
		/// <returns>true if successful, false if parent is null</returns>
		bool Remove();

		/// <summary>
		/// Places a copy of this attribute into the specified node
		/// </summary>
		/// <param name="newParent">the node you wish to copy to</param>
		/// <returns>the newly created IAttribute</returns>
		IAttribute CopyTo(NsNode newParent);

		/// <summary>
		/// Returns a string representation of the desired attribute
		/// </summary>
		/// <param name="format">The desired format. Typically a numeric format specifier
		/// Will throw a format exception if the format is invalid</param>
		/// <returns>The formatted string</returns>
		string ToString(string format);	
	}
}
