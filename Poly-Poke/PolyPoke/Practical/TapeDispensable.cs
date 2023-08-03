using System;
using Ossify;
using Unity.Collections;
using UnityEngine;

namespace PolyPoke
{
    [RequireComponent(typeof(LineRenderer))]
    public class TapeDispensable : Whizz 
    {
        [SerializeField] private float expireInSeconds = 15f;
        [SerializeField] private int pointCount = 100;

        private int index = 0;
        private int count = 0;
        private bool returnWhenDone = false;

        private LineRenderer lineRenderer;

        private NativeArray<Vector3> linePoints;
        private NativeArray<float> pointTimes;
        private NativeArray<Vector3> contiguousPoints;                        

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();            

            linePoints = new NativeArray<Vector3>(pointCount, Allocator.Persistent);
            contiguousPoints = new NativeArray<Vector3>(pointCount, Allocator.Persistent);
            pointTimes = new NativeArray<float>(pointCount, Allocator.Persistent);            
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (linePoints.IsCreated) linePoints.Dispose();
            if (contiguousPoints.IsCreated) contiguousPoints.Dispose();
            if (pointTimes.IsCreated) pointTimes.Dispose();        
        }

        /// <inheritdoc />
        protected override void OnReset()
        {
            base.OnReset();

            Clear();
        }

        /// <inheritdoc />
        protected override void OnSuspend()
        {
            base.OnSuspend();

            count = 0;
            index = 0;            
        }

        void Update() 
        {
            float time = Time.time;

            // Chase the tail index as the time expires
            while (count > 0 && time >= pointTimes[index])
            {
                index = (index + 1) % pointCount;
                count--;
            }

            if (count == 0 && returnWhenDone) ReturnToDispenser();
            else UpdatePoints();
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            returnWhenDone = false;
        }

        /// <inheritdoc />
        public override void Begin(Vector3 point) { }

        /// <inheritdoc />
        public override void During(Vector3 point)
        {
            int newIndex = (index + count) % pointCount;

            linePoints[newIndex] = point;
            pointTimes[newIndex] = Time.time + expireInSeconds;
            count++; 
            
            if (count > pointCount) 
            {
                index = (index + (count - pointCount)) % pointCount;
                count = pointCount;
            }
        }

        /// <inheritdoc />
        public override void End(Vector3 point)
        {
            During(point);

            returnWhenDone = true;
        }

        public void Clear()
        {
            count = 0;
            index = 0;
            returnWhenDone = false;
            UpdatePoints();
        }

        private void UpdatePoints()
        {
            lineRenderer.positionCount = count;

            // If the points are not contiguous, copy them to a contiguous array
            if (index + count > pointCount)
            {
                int firstSliceSize = pointCount - index;

                NativeArray<Vector3>.Copy(linePoints, index, contiguousPoints, 0, firstSliceSize);
                NativeArray<Vector3>.Copy(linePoints, 0, contiguousPoints, firstSliceSize, count - firstSliceSize);
         
                lineRenderer.SetPositions(contiguousPoints);
            }
            else
            {
                lineRenderer.SetPositions(new NativeSlice<Vector3>(linePoints, index, count));
            }
        }
    }
}