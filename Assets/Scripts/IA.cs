using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IA : MonoBehaviour {

    public struct Node
    {
        public Vector3 pos;
        public Vector3 parent;
        public int cost;
    }

    Vector3 _pos;
    List<Vector3> path = new List<Vector3>();
    int _costHorizon = 10;
    int _costDiag = 15;
    float _widthBloc;
    float _offset = 0.1f;

	// Use this for initialization
	void Start () {
        StartCoroutine("Movement");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Movement()
    {
        _pos = this.transform.position;
        PathFind();
        while (path.Count > 0)
        {
            Vector3 target = path.First();
            this.transform.position = target;

            path.RemoveAt(0);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void PathFind()
    {
        var obstacles = Manager.Instance.Blocs;

        _widthBloc = obstacles.First().collider.bounds.size.x;
        float zBloc = obstacles.First().collider.bounds.size.z;

        float yMin = obstacles.Aggregate((o1, o2) =>
            Mathf.RoundToInt(o1.transform.position.y) < Mathf.RoundToInt(o2.transform.position.y) ? o1 : o2).transform.position.y;
        float yMax = obstacles.Aggregate((o1, o2) =>
            Mathf.RoundToInt(o1.transform.position.y) > Mathf.RoundToInt(o2.transform.position.y) ? o1 : o2).transform.position.y;

        // Obtain starting point
        float xMin = obstacles.Aggregate((o1, o2) =>
            Mathf.RoundToInt(o1.transform.position.x) < Mathf.RoundToInt(o2.transform.position.x) ? o1 : o2).transform.position.x;
        Vector3 startPoint = obstacles.Where(o =>
            Mathf.RoundToInt(o.transform.position.x) == Mathf.RoundToInt(xMin)).Aggregate((o1, o2) =>
             Mathf.RoundToInt(o1.transform.position.y) > Mathf.RoundToInt(o2.transform.position.y) ? o1 : o2).transform.position;
        startPoint = new Vector3(startPoint.x, startPoint.y + _widthBloc, zBloc);

        // Obtain end point
        float xMax = obstacles.Aggregate((o1, o2) =>
            Mathf.RoundToInt(o1.transform.position.x) > Mathf.RoundToInt(o2.transform.position.x) ? o1 : o2).transform.position.x;
        Vector3 endPoint = obstacles.Where(o =>
            Mathf.RoundToInt(o.transform.position.x) == Mathf.RoundToInt(xMax)).Aggregate((o1, o2) =>
             Mathf.RoundToInt(o1.transform.position.y) > Mathf.RoundToInt(o2.transform.position.y) ? o1 : o2).transform.position;
        endPoint = new Vector3(endPoint.x, endPoint.y + _widthBloc, zBloc);

        List<Vector3> allNodes = new List<Vector3>();

        /*for (float x = (xMin - _offset); x <= xMax + 0.5; x += _offset)
        {
            for (float y = yMin; y <= yMax + _offset + 0.5; y += _offset)
            {
                allNodes.Add(new Vector3(x, y, zBloc));
            }
        }*/

        foreach (var o in obstacles)
        {
            allNodes.Add(new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z));
            allNodes.Add(new Vector3(o.transform.position.x, o.transform.position.y + _widthBloc, o.transform.position.z));
        }

        Node startingNode = new Node();
        startingNode.pos = allNodes.Where(n =>
            n.x >= startPoint.x && n.x <= startPoint.x + _offset
            &&
            n.y >= startPoint.y && n.y <= startPoint.y + _offset).First();
        startingNode.cost = 0;

        var l = obstacles.Select(o => o.transform.position).ToList();

        /********************************************/

        Dictionary<Node, float> openList = new Dictionary<Node, float>();
        openList.Add(startingNode, 0);
        List<Node> closedList = new List<Node>();
        bool foundTheEnd = false;

        foreach (var n in allNodes)
            print(n);

        while(!foundTheEnd && openList.Count > 0)
        {
            var currentNode = openList.Aggregate((n1, n2) =>
                n1.Value < n2.Value ? n1 : n2);

            openList.Remove(currentNode.Key);
            closedList.Add(currentNode.Key);

            List<Node> neighbors = FindPossibleNodes(currentNode.Key, l, allNodes, closedList);

            foreach (var n in neighbors)
            {
                if (!openList.Any(nv => nv.Key.pos == n.pos))
                {
                    float val = n.cost + calculEstimateMovement(n.pos, endPoint);
                    openList.Add(n, val);
                }
                else
                {
                    var nv = openList.Where(p => p.Key.pos == n.pos).First();
                    if (n.cost > nv.Key.cost)
                    {
                        openList.Remove(nv.Key);
                        float val = n.cost + calculEstimateMovement(n.pos, endPoint);
                        openList.Add(n, val);
                    }
                }
            }

            foundTheEnd = (Mathf.Abs(currentNode.Key.pos.x - endPoint.x) < 0.1
                &&
                Mathf.Abs(currentNode.Key.pos.y - endPoint.y) < 0.1) ? true : false;
        }

        Node node = closedList.Last();
        List<Vector3> pos = new List<Vector3>();
        
        while (node.cost != 0)
        {
            pos.Add(node.parent);
            node = closedList.Where(n => n.pos == node.parent).First();
        }
        pos.Reverse();
        path = pos;
    }

    List<Node> FindPossibleNodes(Node current, List<Vector3> obstacles, List<Vector3> allNodes, List<Node> closed)
    {
        bool obstacleRight = obstacles.Any(o =>
            (o.x >= current.pos.x + _offset && o.x <= current.pos.x + _offset * 2)
            &&
            (o.y >= current.pos.y && o.y <= current.pos.y + _offset)
            );
        bool obstacleLeft = obstacles.Any(o =>
            (o.x <= current.pos.x - _offset && o.x <= current.pos.x - _offset * 2)
            &&
            (o.y >= current.pos.y && o.y <= current.pos.y + _offset)
            );
        bool obstacleTop = obstacles.Any(o =>
            (o.x >= current.pos.x && o.x <= current.pos.x + _offset)
            &&
            (o.y >= current.pos.y + _offset && o.y <= current.pos.y + _offset * 2)
            );
        bool obstacleBottom = obstacles.Any(o =>
            (o.x >= current.pos.x && o.x <= current.pos.x + _offset)
            &&
            (o.y >= current.pos.y - _offset && o.y <= current.pos.y - _offset * 2)
            );

        List<Node> returnList = new List<Node>();
        
        Node addNode = new Node();

        if (!obstacleRight)
        {
            addNode.pos = new Vector3(current.pos.x + _offset, current.pos.y, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);
        }

        if (!obstacleLeft)
        {
            addNode.pos = new Vector3(current.pos.x - _offset, current.pos.y, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);
        }

        if (!obstacleTop)
        {
            addNode.pos = new Vector3(current.pos.x, current.pos.y + _offset, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);

            if (!obstacleRight)
            {
                addNode.pos = new Vector3(current.pos.x + _offset, current.pos.y + _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }

            if (!obstacleLeft)
            {
                addNode.pos = new Vector3(current.pos.x - _offset, current.pos.y + _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }
        }

        if (!obstacleBottom)
        {
            addNode.pos = new Vector3(current.pos.x, current.pos.y - _offset, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);

            if (!obstacleRight)
            {
                addNode.pos = new Vector3(current.pos.x + _offset, current.pos.y - _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }

            if (!obstacleLeft)
            {
                addNode.pos = new Vector3(current.pos.x - _offset, current.pos.y - _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }
        }

        return returnList.Where(n => !(closed.Any(n2 => n.pos == n2.pos))).ToList();
    }

    float calculEstimateMovement(Vector3 current, Vector3 target)
    {
        float xDistance = Mathf.Abs(current.x - target.x);
        float yDistance = Mathf.Abs(current.y - target.y);

        if (xDistance < yDistance)
        {
            return _costDiag * yDistance + _costHorizon * (xDistance - yDistance);
        }
        else
        {
            return _costDiag * xDistance + _costHorizon * (yDistance - xDistance);
        }
    }
}
